using System.Collections.Concurrent;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Patients;

namespace GIPractice.Api.Patients;

public sealed class InMemoryPatientsStore : IPatientsStore
{
    private sealed record StoredPatient(
        PatientId Id,
        string LastName,
        string FirstName,
        string? FathersName,
        DateTime? BirthDate,
        string? PersonalNumber,
        string? Gender,
        string? PhoneNumber,
        string? Email,
        string? Address,
        bool HasHadCA,
        bool HasHadIBD,
        bool HasPendingBiopsies,
        bool HasScheduledEndo,
        byte[]? PhotoBytes,
        string? PhotoContentType,
        byte[]? RowVersion);

    private readonly ConcurrentDictionary<int, StoredPatient> _patients = new();
    private int _nextId = 1;

    public InMemoryPatientsStore()
    {
        // Minimal seed data so the UI has something to show during scaffolding.
        CreateInternal(new PatientUpsertRequestDto(
            Id: null,
            LastName: "Papadopoulos",
            FirstName: "Giorgos",
            FathersName: "Dimitrios",
            BirthDate: new DateTime(1972, 3, 14),
            PersonalNumber: "12345678901",
            Gender: "M",
            PhoneNumber: "+30 210 0000000",
            Email: "giorgos@example.com",
            Address: "Athens",
            HasHadCA: false,
            HasHadIBD: true,
            HasPendingBiopsies: false,
            HasScheduledEndo: true,
            PhotoBytes: null,
            PhotoContentType: null,
            RowVersion: null));

        CreateInternal(new PatientUpsertRequestDto(
            Id: null,
            LastName: "Konstantinou",
            FirstName: "Maria",
            FathersName: "Nikolaos",
            BirthDate: new DateTime(1985, 11, 2),
            PersonalNumber: "98765432109",
            Gender: "F",
            PhoneNumber: "+30 694 0000000",
            Email: "maria@example.com",
            Address: "Piraeus",
            HasHadCA: false,
            HasHadIBD: false,
            HasPendingBiopsies: true,
            HasScheduledEndo: false,
            PhotoBytes: null,
            PhotoContentType: null,
            RowVersion: null));
    }

    public Task<ResultDto<PagedResultDto<PatientListItemDto>>> SearchAsync(
        PatientSearchRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var paging = request.Paging ?? new PagedRequestDto();
        if (paging.Page < 1)
            return Task.FromResult(ResultDto<PagedResultDto<PatientListItemDto>>.Fail(new ErrorDto("invalid", "Page must be >= 1.")));
        if (paging.PageSize is < 1 or > 500)
            return Task.FromResult(ResultDto<PagedResultDto<PatientListItemDto>>.Fail(new ErrorDto("invalid", "PageSize must be between 1 and 500.")));

        IEnumerable<StoredPatient> q = _patients.Values;

        q = ApplyStringFilter(q, p => p.FirstName, request.FirstName);
        q = ApplyStringFilter(q, p => p.LastName, request.LastName);
        q = ApplyStringFilter(q, p => p.FathersName, request.FathersName);
        q = ApplyStringFilter(q, p => p.PersonalNumber, request.PersonalNumber);
        q = ApplyStringFilter(q, p => p.PhoneNumber, request.Phone);
        q = ApplyStringFilter(q, p => p.Email, request.Email);

        q = ApplyTriState(q, p => p.HasHadCA, request.HasHadCA);
        q = ApplyTriState(q, p => p.HasHadIBD, request.HasHadIBD);
        q = ApplyTriState(q, p => p.HasPendingBiopsies, request.HasPendingBiopsies);
        q = ApplyTriState(q, p => p.HasScheduledEndo, request.HasScheduledEndo);

        var total = q.Count();

        if (paging.Sort is not null)
        {
            var field = paging.Sort.Field?.Trim() ?? "";
            var desc = paging.Sort.Desc;

            q = (field.ToLowerInvariant()) switch
            {
                "lastname" => desc ? q.OrderByDescending(p => p.LastName) : q.OrderBy(p => p.LastName),
                "firstname" => desc ? q.OrderByDescending(p => p.FirstName) : q.OrderBy(p => p.FirstName),
                "birthdate" => desc ? q.OrderByDescending(p => p.BirthDate) : q.OrderBy(p => p.BirthDate),
                "personalnumber" => desc ? q.OrderByDescending(p => p.PersonalNumber) : q.OrderBy(p => p.PersonalNumber),
                _ => null
            } ?? Enumerable.Empty<StoredPatient>();

            if (!q.Any() && total > 0)
                return Task.FromResult(ResultDto<PagedResultDto<PatientListItemDto>>.Fail(new ErrorDto("invalid", $"Unknown sort field '{paging.Sort.Field}'.")));
        }
        else
        {
            q = q.OrderBy(p => p.LastName).ThenBy(p => p.FirstName);
        }

        var items = q
            .Skip((paging.Page - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .Select(p => new PatientListItemDto(
                p.Id,
                p.LastName,
                p.FirstName,
                p.FathersName,
                p.BirthDate,
                p.PersonalNumber,
                p.PhoneNumber,
                p.Email))
            .ToArray();

        var result = new PagedResultDto<PatientListItemDto>(items, total, paging.Page, paging.PageSize);
        return Task.FromResult(ResultDto<PagedResultDto<PatientListItemDto>>.Ok(result));
    }

    public Task<ResultDto<PatientDetailsDto>> GetDetailsAsync(PatientId patientId, CancellationToken cancellationToken = default)
    {
        if (!_patients.TryGetValue(patientId.Value, out var p))
            return Task.FromResult(ResultDto<PatientDetailsDto>.Fail(new ErrorDto("not_found", "Patient not found.")));

        var dto = new PatientDetailsDto(
            p.Id,
            p.LastName,
            p.FirstName,
            p.FathersName,
            p.BirthDate,
            p.PersonalNumber,
            p.Gender,
            p.PhoneNumber,
            p.Email,
            p.Address,
            p.HasHadCA,
            p.HasHadIBD,
            p.HasPendingBiopsies,
            p.HasScheduledEndo,
            p.PhotoBytes,
            p.PhotoContentType,
            p.RowVersion);

        return Task.FromResult(ResultDto<PatientDetailsDto>.Ok(dto));
    }

    public Task<ResultDto<PatientId>> CreateAsync(PatientUpsertRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request.Id is not null)
            return Task.FromResult(ResultDto<PatientId>.Fail(new ErrorDto("invalid", "Id must be null when creating a patient.")));

        if (string.IsNullOrWhiteSpace(request.LastName) || string.IsNullOrWhiteSpace(request.FirstName))
            return Task.FromResult(ResultDto<PatientId>.Fail(new ErrorDto("validation", "FirstName and LastName are required.")));

        var id = CreateInternal(request);
        return Task.FromResult(ResultDto<PatientId>.Ok(id));
    }

    public Task<ResultDto<bool>> UpdateAsync(PatientUpsertRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request.Id is null)
            return Task.FromResult(ResultDto<bool>.Fail(new ErrorDto("invalid", "Id is required when updating a patient.")));

        var id = request.Id.Value;

        if (!_patients.TryGetValue(id.Value, out var existing))
            return Task.FromResult(ResultDto<bool>.Fail(new ErrorDto("not_found", "Patient not found.")));

        if (request.RowVersion is not null && existing.RowVersion is not null && !request.RowVersion.SequenceEqual(existing.RowVersion))
            return Task.FromResult(ResultDto<bool>.Fail(new ErrorDto("conflict", "RowVersion conflict.")));

        var updated = existing with
        {
            LastName = request.LastName,
            FirstName = request.FirstName,
            FathersName = request.FathersName,
            BirthDate = request.BirthDate,
            PersonalNumber = request.PersonalNumber,
            Gender = request.Gender,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            Address = request.Address,
            HasHadCA = request.HasHadCA,
            HasHadIBD = request.HasHadIBD,
            HasPendingBiopsies = request.HasPendingBiopsies,
            HasScheduledEndo = request.HasScheduledEndo,
            PhotoBytes = request.PhotoBytes,
            PhotoContentType = request.PhotoContentType,
            RowVersion = NewRowVersion()
        };

        _patients[id.Value] = updated;
        return Task.FromResult(ResultDto<bool>.Ok(true));
    }

    public Task<ResultDto<bool>> DeleteAsync(PatientId patientId, CancellationToken cancellationToken = default)
    {
        if (!_patients.TryRemove(patientId.Value, out _))
            return Task.FromResult(ResultDto<bool>.Fail(new ErrorDto("not_found", "Patient not found.")));

        return Task.FromResult(ResultDto<bool>.Ok(true));
    }

    private PatientId CreateInternal(PatientUpsertRequestDto request)
    {
        var id = new PatientId(_nextId++);
        var stored = new StoredPatient(
            id,
            request.LastName,
            request.FirstName,
            request.FathersName,
            request.BirthDate,
            request.PersonalNumber,
            request.Gender,
            request.PhoneNumber,
            request.Email,
            request.Address,
            request.HasHadCA,
            request.HasHadIBD,
            request.HasPendingBiopsies,
            request.HasScheduledEndo,
            request.PhotoBytes,
            request.PhotoContentType,
            NewRowVersion());

        _patients[id.Value] = stored;
        return id;
    }

    private static byte[] NewRowVersion() => Guid.NewGuid().ToByteArray();

    private static IEnumerable<StoredPatient> ApplyStringFilter(
        IEnumerable<StoredPatient> q,
        Func<StoredPatient, string?> selector,
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return q;

        var needle = value.Trim();
        return q.Where(p => (selector(p) ?? string.Empty).Contains(needle, StringComparison.OrdinalIgnoreCase));
    }

    private static IEnumerable<StoredPatient> ApplyTriState(
        IEnumerable<StoredPatient> q,
        Func<StoredPatient, bool> selector,
        bool? state)
    {
        if (state is null)
            return q;

        return state.Value ? q.Where(p => selector(p)) : q.Where(p => !selector(p));
    }
}
