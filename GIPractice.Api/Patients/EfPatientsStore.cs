using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Patients;
using GIPractice.Core.Entities;
using GIPractice.Core.Enums;
using GIPractice.Core.ValueObjects;
using GIPractice.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace GIPractice.Api.Patients;

public sealed class EfPatientsStore : IPatientsStore
{
    private readonly AppDbContext _db;

    public EfPatientsStore(AppDbContext db) => _db = db;

    public async Task<ResultDto<PagedResultDto<PatientListItemDto>>> SearchAsync(
        PatientSearchRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var paging = request.Paging ?? new PagedRequestDto();

        if (paging.Page < 1)
            return ResultDto<PagedResultDto<PatientListItemDto>>.Fail("invalid", "Page must be >= 1.");

        if (paging.PageSize is < 1 or > 500)
            return ResultDto<PagedResultDto<PatientListItemDto>>.Fail("invalid", "PageSize must be between 1 and 500.");

        IQueryable<Patient> q = _db.Patients.AsNoTracking();

        q = ApplyLike(q, p => p.FirstName, request.FirstName);
        q = ApplyLike(q, p => p.LastName, request.LastName);
        q = ApplyLike(q, p => p.FathersName, request.FathersName);
        q = ApplyLike(q, p => p.Email!, request.Email);
        q = ApplyLike(q, p => p.PhoneNumber!, request.Phone);

        if (!string.IsNullOrWhiteSpace(request.PersonalNumber))
        {
            if (!PersonalNumber.TryCreate(request.PersonalNumber, out var pn))
                return ResultDto<PagedResultDto<PatientListItemDto>>.Fail("validation", "Invalid PersonalNumber (expected 12 digits).");

            // Equality (safe + translates via value converter)
            q = q.Where(p => p.PersonalNumber == pn);
        }

        // NOTE: DTO-only tri-state fields (HasHadCA/IBD/etc) do NOT exist on Patient entity yet.
        // For now we ignore those filters (no schema churn). We'll wire them once the columns exist.

        var total = await q.CountAsync(cancellationToken);

        q = ApplySort(q, paging.Sort, total, out var sortError);
        if (sortError is not null)
            return ResultDto<PagedResultDto<PatientListItemDto>>.Fail("invalid", sortError);

        var rows = await q
            .Skip((paging.Page - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .ToListAsync(cancellationToken);

        var items = rows
            .Select(p => new PatientListItemDto(
                new PatientId(p.Id),
                p.LastName,
                p.FirstName,
                string.IsNullOrWhiteSpace(p.FathersName) ? null : p.FathersName,
                p.BirthDay == DateTime.MinValue ? null : p.BirthDay,
                p.PersonalNumber.Value,
                p.PhoneNumber,
                p.Email))
            .ToArray();

        return ResultDto<PagedResultDto<PatientListItemDto>>.Ok(
            new PagedResultDto<PatientListItemDto>(items, total, paging.Page, paging.PageSize));
    }

    public async Task<ResultDto<PatientDetailsDto>> GetDetailsAsync(
        PatientId patientId,
        CancellationToken cancellationToken = default)
    {
        var p = await _db.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == patientId.Value, cancellationToken);

        if (p is null)
            return ResultDto<PatientDetailsDto>.Fail("not_found", "Patient not found.");

        // DTO-only fields not yet in schema => defaults for now
        var dto = new PatientDetailsDto(
            new PatientId(p.Id),
            p.LastName,
            p.FirstName,
            string.IsNullOrWhiteSpace(p.FathersName) ? null : p.FathersName,
            p.BirthDay == DateTime.MinValue ? null : p.BirthDay,
            p.PersonalNumber.Value,
            GenderToString(p.Gender),
            p.PhoneNumber,
            p.Email,
            p.Address,
            HasHadCA: false,
            HasHadIBD: false,
            HasPendingBiopsies: false,
            HasScheduledEndo: false,
            PhotoBytes: null,
            PhotoContentType: null,
            RowVersion: MakeRowVersion(p));

        return ResultDto<PatientDetailsDto>.Ok(dto);
    }

    public async Task<ResultDto<PatientId>> CreateAsync(
        PatientUpsertRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (request.Id is not null)
            return ResultDto<PatientId>.Fail("invalid", "Id must be null when creating a patient.");

        if (string.IsNullOrWhiteSpace(request.LastName) || string.IsNullOrWhiteSpace(request.FirstName))
            return ResultDto<PatientId>.Fail("validation", "FirstName and LastName are required.");

        if (string.IsNullOrWhiteSpace(request.PersonalNumber))
            return ResultDto<PatientId>.Fail("validation", "PersonalNumber is required.");

        if (!PersonalNumber.TryCreate(request.PersonalNumber, out var pn))
            return ResultDto<PatientId>.Fail("validation", "Invalid PersonalNumber (expected 12 digits).");

        // Cheap pre-check for unique PN (still keep DbUpdate catch below)
        var pnTaken = await _db.Patients.AnyAsync(p => p.PersonalNumber == pn, cancellationToken);
        if (pnTaken)
            return ResultDto<PatientId>.Fail("conflict", "PersonalNumber already exists.");

        var entity = new Patient
        {
            LastName = TrimMax(request.LastName, 30),
            FirstName = TrimMax(request.FirstName, 30),
            FathersName = TrimMax(request.FathersName ?? "", 30),

            PersonalNumber = pn,
            BirthDay = request.BirthDate ?? DateTime.MinValue,
            Gender = ParseGender(request.Gender),

            Email = TrimMaxNullable(request.Email, 100),
            PhoneNumber = TrimMaxNullable(request.PhoneNumber, 20),
            Address = TrimMaxNullable(request.Address, 250),
        };

        _db.Patients.Add(entity);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            // Most likely unique PN violation
            return ResultDto<PatientId>.Fail("conflict", "Could not create patient (possible duplicate PersonalNumber).");
        }

        return ResultDto<PatientId>.Ok(new PatientId(entity.Id));
    }

    public async Task<ResultDto<bool>> UpdateAsync(
        PatientUpsertRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (request.Id is null)
            return ResultDto<bool>.Fail("invalid", "Id is required when updating a patient.");

        var id = request.Id.Value.Value;

        var entity = await _db.Patients.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (entity is null)
            return ResultDto<bool>.Fail("not_found", "Patient not found.");

        if (request.RowVersion is not null && !request.RowVersion.SequenceEqual(MakeRowVersion(entity)))
            return ResultDto<bool>.Fail("conflict", "RowVersion conflict.");

        if (string.IsNullOrWhiteSpace(request.PersonalNumber))
            return ResultDto<bool>.Fail("validation", "PersonalNumber is required.");

        if (!PersonalNumber.TryCreate(request.PersonalNumber, out var pn))
            return ResultDto<bool>.Fail("validation", "Invalid PersonalNumber (expected 12 digits).");

        // Unique PN check (exclude self)
        var pnTaken = await _db.Patients.AnyAsync(p => p.Id != id && p.PersonalNumber == pn, cancellationToken);
        if (pnTaken)
            return ResultDto<bool>.Fail("conflict", "PersonalNumber already exists.");

        entity.LastName = TrimMax(request.LastName, 30);
        entity.FirstName = TrimMax(request.FirstName, 30);
        entity.FathersName = TrimMax(request.FathersName ?? "", 30);

        entity.PersonalNumber = pn;
        entity.BirthDay = request.BirthDate ?? entity.BirthDay; // keep existing if null (or change if you prefer)
        entity.Gender = ParseGender(request.Gender);

        entity.Email = TrimMaxNullable(request.Email, 100);
        entity.PhoneNumber = TrimMaxNullable(request.PhoneNumber, 20);
        entity.Address = TrimMaxNullable(request.Address, 250);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return ResultDto<bool>.Fail("conflict", "Could not update patient (possible duplicate PersonalNumber).");
        }

        return ResultDto<bool>.Ok(true);
    }

    public async Task<ResultDto<bool>> DeleteAsync(
        PatientId patientId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _db.Patients.FirstOrDefaultAsync(p => p.Id == patientId.Value, cancellationToken);
        if (entity is null)
            return ResultDto<bool>.Fail("not_found", "Patient not found.");

        _db.Patients.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return ResultDto<bool>.Ok(true);
    }

    private static IQueryable<Patient> ApplyLike(
        IQueryable<Patient> q,
        System.Linq.Expressions.Expression<Func<Patient, string>> selector,
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return q;

        var needle = value.Trim();
        return q.Where(p => EF.Functions.Like(EF.Property<string>(p, GetMemberName(selector)), $"%{needle}%"));
    }

    private static string GetMemberName(System.Linq.Expressions.Expression<Func<Patient, string>> expr)
    {
        // expr = p => p.FirstName
        if (expr.Body is System.Linq.Expressions.MemberExpression m)
            return m.Member.Name;

        throw new InvalidOperationException("Only simple member expressions are supported.");
    }

    private static IQueryable<Patient> ApplySort(
        IQueryable<Patient> q,
        SortDto? sort,
        int total,
        out string? error)
    {
        error = null;

        if (sort is null)
            return q.OrderBy(p => p.LastName).ThenBy(p => p.FirstName);

        var field = (sort.Field ?? "").Trim().ToLowerInvariant();
        var desc = sort.Desc;

        return field switch
        {
            "lastname" => desc ? q.OrderByDescending(p => p.LastName) : q.OrderBy(p => p.LastName),
            "firstname" => desc ? q.OrderByDescending(p => p.FirstName) : q.OrderBy(p => p.FirstName),
            "birthdate" or "birthday" => desc ? q.OrderByDescending(p => p.BirthDay) : q.OrderBy(p => p.BirthDay),
            "personalnumber" => desc ? q.OrderByDescending(p => p.PersonalNumber) : q.OrderBy(p => p.PersonalNumber),

            _ => total > 0
                ? SetErrorAndReturn(q, out error, $"Unknown sort field '{sort.Field}'.")
                : q
        };
    }

    private static IQueryable<Patient> SetErrorAndReturn(IQueryable<Patient> q, out string? error, string message)
    {
        error = message;
        return q;
    }

    private static Gender ParseGender(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return Gender.None;

        var v = s.Trim().ToLowerInvariant();
        return v switch
        {
            "m" or "male" => Gender.Male,
            "f" or "female" => Gender.Female,
            "o" or "other" => Gender.Other,
            _ => Gender.None
        };
    }

    private static string? GenderToString(Gender g) => g switch
    {
        Gender.Male => "M",
        Gender.Female => "F",
        Gender.Other => "O",
        _ => null
    };

    private static byte[] MakeRowVersion(Patient p)
        => BitConverter.GetBytes((p.UpdatedAtUtc ?? p.CreatedAtUtc).Ticks);

    private static string TrimMax(string s, int max)
    {
        s = (s ?? "").Trim();
        return s.Length <= max ? s : s[..max];
    }

    private static string? TrimMaxNullable(string? s, int max)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        s = s.Trim();
        return s.Length <= max ? s : s[..max];
    }
}
