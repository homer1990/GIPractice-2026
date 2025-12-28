using GIPractice.Contracts.Common;
using GIPractice.Contracts.Encounters;
using GIPractice.Contracts.Ids;
using GIPractice.Infrastructure;
using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Encounters;

public sealed class EfEncountersStore(AppDbContext db) : IEncountersStore
{
    private readonly AppDbContext _db = db;

    public async Task<PagedResultDto<EncounterListItemDto>> SearchAsync(EncounterSearchRequestDto request, CancellationToken ct)
    {
        // We only have one encounter type in DB right now: Visit => EncounterTypeId=1.
        if (request.EncounterTypeId is { } et && et.Value != 1)
            return new PagedResultDto<EncounterListItemDto>(Array.Empty<EncounterListItemDto>(), 0, request.Paging?.Page ?? 1, request.Paging?.PageSize ?? 50);

        // We currently do NOT store IsUrgent in Visit; treat all as false.
        if (request.IsUrgent is true)
            return new PagedResultDto<EncounterListItemDto>(Array.Empty<EncounterListItemDto>(), 0, request.Paging?.Page ?? 1, request.Paging?.PageSize ?? 50);

        var now = DateTime.UtcNow;

        IQueryable<Visit> q = _db.Visits.AsNoTracking();

        if (request.PatientId is { } pid)
            q = q.Where(v => v.PatientId == pid.Value);

        if (request.DateFrom is { } df)
        {
            var fromUtc = df.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            q = q.Where(v => v.DateOfVisitUtc >= fromUtc);
        }

        if (request.DateTo is { } dt)
        {
            var toUtc = dt.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
            q = q.Where(v => v.DateOfVisitUtc <= toUtc);
        }

        if (request.Status is { } st)
        {
            // Derived status:
            // Planned => DateOfVisitUtc > now
            // Completed => DateOfVisitUtc <= now
            // InProgress/Cancelled => not representable yet => empty
            q = st switch
            {
                EncounterStatus.Planned => q.Where(v => v.DateOfVisitUtc > now),
                EncounterStatus.Completed => q.Where(v => v.DateOfVisitUtc <= now),
                _ => q.Where(_ => false)
            };
        }

        // Paging
        var page = request.Paging?.Page ?? 1;
        var pageSize = request.Paging?.PageSize ?? 50;
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 50;

        // Sorting (minimal, safe defaults)
        q = ApplySort(q, request.Paging?.Sort);

        var total = await q.CountAsync(ct);

        var rows = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = rows.Select(v =>
        {
            var status = v.DateOfVisitUtc > now ? EncounterStatus.Planned : EncounterStatus.Completed;

            return new EncounterListItemDto(
                new EncounterId(v.Id),
                new PatientId(v.PatientId),
                new EncounterTypeId(1),
                "Visit",
                v.DateOfVisitUtc,
                status,
                IsUrgent: false
            );
        }).ToArray();

        return new PagedResultDto<EncounterListItemDto>(items, total, page, pageSize);
    }

    public async Task<EncounterDetailsDto?> GetAsync(EncounterId id, CancellationToken ct)
    {
        var v = await _db.Visits.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id.Value, ct);
        if (v is null) return null;

        var now = DateTime.UtcNow;
        var status = v.DateOfVisitUtc > now ? EncounterStatus.Planned : EncounterStatus.Completed;

        return new EncounterDetailsDto(
            new EncounterId(v.Id),
            new PatientId(v.PatientId),
            new EncounterTypeId(1),
            StartUtc: v.DateOfVisitUtc,
            EndUtc: null,
            Status: status,
            IsUrgent: false,
            Notes: v.Notes,
            RowVersion: MakeRowVersion(v)
        );
    }

    public async Task<ResultDto<EncounterId>> CreateAsync(EncounterUpsertRequestDto request, CancellationToken ct)
    {
        // We only support Visit right now.
        if (request.EncounterTypeId.Value != 1)
            return ResultDto<EncounterId>.Fail("invalid", "Only EncounterTypeId=1 (Visit) is currently supported.");

        // IsUrgent/Status/EndUtc not stored yet -> accepted but ignored.

        // Patient must exist (nice error instead of FK explosion)
        var patientExists = await _db.Patients.AnyAsync(p => p.Id == request.PatientId.Value, ct);
        if (!patientExists)
            return ResultDto<EncounterId>.Fail("not_found", "Patient not found.");

        var entity = new Visit
        {
            PatientId = request.PatientId.Value,
            DateOfVisitUtc = request.StartUtc,
            Notes = request.Notes,
            AppointmentId = null
        };

        _db.Visits.Add(entity);
        await _db.SaveChangesAsync(ct);

        return ResultDto<EncounterId>.Ok(new EncounterId(entity.Id));
    }

    public async Task<ResultDto<bool>> UpdateAsync(EncounterUpsertRequestDto request, CancellationToken ct)
    {
        if (request.Id is null)
            return ResultDto<bool>.Fail("invalid", "Id is required for update.");

        if (request.EncounterTypeId.Value != 1)
            return ResultDto<bool>.Fail("invalid", "Only EncounterTypeId=1 (Visit) is currently supported.");

        var entity = await _db.Visits.FirstOrDefaultAsync(v => v.Id == request.Id.Value.Value, ct);
        if (entity is null)
            return ResultDto<bool>.Fail("not_found", "Encounter not found.");

        if (request.RowVersion is not null && !request.RowVersion.SequenceEqual(MakeRowVersion(entity)))
            return ResultDto<bool>.Fail("conflict", "RowVersion conflict.");

        var patientExists = await _db.Patients.AnyAsync(p => p.Id == request.PatientId.Value, ct);
        if (!patientExists)
            return ResultDto<bool>.Fail("not_found", "Patient not found.");

        entity.PatientId = request.PatientId.Value;
        entity.DateOfVisitUtc = request.StartUtc;
        entity.Notes = request.Notes;

        await _db.SaveChangesAsync(ct);
        return ResultDto<bool>.Ok(true);
    }

    public async Task<ResultDto<bool>> DeleteAsync(EncounterId id, CancellationToken ct)
    {
        var entity = await _db.Visits.FirstOrDefaultAsync(v => v.Id == id.Value, ct);
        if (entity is null)
            return ResultDto<bool>.Fail("not_found", "Encounter not found.");

        _db.Visits.Remove(entity);          // will soft-delete due to your SaveChanges hook
        await _db.SaveChangesAsync(ct);

        return ResultDto<bool>.Ok(true);
    }

    private static IQueryable<Visit> ApplySort(IQueryable<Visit> q, SortDto? sort)
    {
        if (sort is null)
            return q.OrderByDescending(v => v.DateOfVisitUtc).ThenByDescending(v => v.Id);

        var field = (sort.Field ?? "").Trim().ToLowerInvariant();
        var desc = sort.Desc;

        return field switch
        {
            "startutc" or "date" or "dateofvisitutc" =>
                desc ? q.OrderByDescending(v => v.DateOfVisitUtc) : q.OrderBy(v => v.DateOfVisitUtc),

            "id" =>
                desc ? q.OrderByDescending(v => v.Id) : q.OrderBy(v => v.Id),

            "patientid" =>
                desc ? q.OrderByDescending(v => v.PatientId) : q.OrderBy(v => v.PatientId),

            _ => q.OrderByDescending(v => v.DateOfVisitUtc).ThenByDescending(v => v.Id)
        };
    }

    private static byte[] MakeRowVersion(Visit v)
        => BitConverter.GetBytes((v.UpdatedAtUtc ?? v.CreatedAtUtc).Ticks);
}
