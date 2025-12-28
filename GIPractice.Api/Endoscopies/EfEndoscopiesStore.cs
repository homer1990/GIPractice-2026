using GIPractice.Contracts.Common;
using GIPractice.Contracts.Endoscopies;
using GIPractice.Contracts.Ids;
using GIPractice.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Endoscopies;

public sealed class EfEndoscopiesStore : IEndoscopiesStore
{
    private readonly AppDbContext _db;

    public EfEndoscopiesStore(AppDbContext db) => _db = db;

    public async Task<PagedResultDto<EndoscopyListItemDto>> SearchAsync(EndoscopySearchRequestDto request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        // If caller asks for a status we can't represent -> return empty (explicit, predictable)
        if (request.Status is EndoscopyStatus.InProgress or EndoscopyStatus.Cancelled)
            return EmptyPage(request);

        IQueryable<GIPractice.Core.Entities.Endoscopy> q = _db.Endoscopies.AsNoTracking();

        if (request.PatientId is { } pid)
            q = q.Where(e => e.PatientId == pid.Value);

        if (request.EncounterId is { } encId)
            q = q.Where(e => e.VisitId == encId.Value);

        if (request.EndoscopyTypeId is { } tid)
            q = q.Where(e => (int)e.Type == tid.Value);

        if (request.IsUrgent is { } urg)
            q = q.Where(e => e.IsUrgent == urg);

        if (request.DateFrom is { } df)
        {
            var fromUtc = df.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            q = q.Where(e => e.PerformedAtUtc >= fromUtc);
        }

        if (request.DateTo is { } dt)
        {
            var toUtc = dt.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
            q = q.Where(e => e.PerformedAtUtc <= toUtc);
        }

        if (request.Status is { } st)
        {
            q = st switch
            {
                EndoscopyStatus.Planned => q.Where(e => e.PerformedAtUtc > now),
                EndoscopyStatus.Completed => q.Where(e => e.PerformedAtUtc <= now),
                _ => q
            };
        }

        // Paging
        var page = request.Paging?.Page ?? 1;
        var pageSize = request.Paging?.PageSize ?? 50;
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 50;

        // Sort
        q = ApplySort(q, request.Paging?.Sort);

        var total = await q.CountAsync(ct);

        var rows = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = rows.Select(e =>
        {
            var status = e.PerformedAtUtc > now ? EndoscopyStatus.Planned : EndoscopyStatus.Completed;

            return new EndoscopyListItemDto(
                new EndoscopyId(e.Id),
                new PatientId(e.PatientId),
                new EncounterId(e.VisitId),
                new EndoscopyTypeId((int)e.Type),
                e.Type.ToString(),
                e.PerformedAtUtc,
                status,
                e.IsUrgent
            );
        }).ToArray();

        return new PagedResultDto<EndoscopyListItemDto>(items, total, page, pageSize);
    }

    public async Task<EndoscopyDetailsDto?> GetAsync(EndoscopyId id, CancellationToken ct)
    {
        var e = await _db.Endoscopies.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id.Value, ct);
        if (e is null) return null;

        var now = DateTime.UtcNow;
        var status = e.PerformedAtUtc > now ? EndoscopyStatus.Planned : EndoscopyStatus.Completed;

        return new EndoscopyDetailsDto(
            new EndoscopyId(e.Id),
            new PatientId(e.PatientId),
            new EncounterId(e.VisitId),
            new EndoscopyTypeId((int)e.Type),
            StartUtc: e.PerformedAtUtc,
            EndUtc: null,
            Status: status,
            IsUrgent: e.IsUrgent,
            Notes: e.Notes,
            RowVersion: MakeRowVersion(e)
        );
    }

    public async Task<ResultDto<EndoscopyId>> CreateAsync(EndoscopyUpsertRequestDto request, CancellationToken ct)
    {
        if (request.Id is not null)
            return ResultDto<EndoscopyId>.Fail("invalid", "Id must be null when creating.");

        if (!Enum.IsDefined(typeof(GIPractice.Core.Enums.EndoscopyType), request.EndoscopyTypeId.Value))
            return ResultDto<EndoscopyId>.Fail("validation", "Unknown EndoscopyTypeId.");

        // We can't persist InProgress/Cancelled yet -> reject explicitly
        if (request.Status is EndoscopyStatus.InProgress or EndoscopyStatus.Cancelled)
            return ResultDto<EndoscopyId>.Fail("invalid", "Status not supported yet (no DB column).");

        var patientExists = await _db.Patients.AnyAsync(p => p.Id == request.PatientId.Value, ct);
        if (!patientExists)
            return ResultDto<EndoscopyId>.Fail("not_found", "Patient not found.");

        var visit = await _db.Visits.AsNoTracking().FirstOrDefaultAsync(v => v.Id == request.EncounterId.Value, ct);
        if (visit is null)
            return ResultDto<EndoscopyId>.Fail("not_found", "Encounter/Visit not found.");

        if (visit.PatientId != request.PatientId.Value)
            return ResultDto<EndoscopyId>.Fail("validation", "Encounter does not belong to patient.");

        var entity = new GIPractice.Core.Entities.Endoscopy
        {
            PatientId = request.PatientId.Value,
            VisitId = request.EncounterId.Value,
            Type = (GIPractice.Core.Enums.EndoscopyType)request.EndoscopyTypeId.Value,
            PerformedAtUtc = request.StartUtc,
            IsUrgent = request.IsUrgent,
            Notes = request.Notes
        };

        _db.Endoscopies.Add(entity);
        await _db.SaveChangesAsync(ct);

        return ResultDto<EndoscopyId>.Ok(new EndoscopyId(entity.Id));
    }

    public async Task<ResultDto<bool>> UpdateAsync(EndoscopyUpsertRequestDto request, CancellationToken ct)
    {
        if (request.Id is null)
            return ResultDto<bool>.Fail("invalid", "Id is required for update.");

        if (!Enum.IsDefined(typeof(GIPractice.Core.Enums.EndoscopyType), request.EndoscopyTypeId.Value))
            return ResultDto<bool>.Fail("validation", "Unknown EndoscopyTypeId.");

        if (request.Status is EndoscopyStatus.InProgress or EndoscopyStatus.Cancelled)
            return ResultDto<bool>.Fail("invalid", "Status not supported yet (no DB column).");

        var entity = await _db.Endoscopies.FirstOrDefaultAsync(e => e.Id == request.Id.Value.Value, ct);
        if (entity is null)
            return ResultDto<bool>.Fail("not_found", "Endoscopy not found.");

        if (request.RowVersion is not null && !request.RowVersion.SequenceEqual(MakeRowVersion(entity)))
            return ResultDto<bool>.Fail("conflict", "RowVersion conflict.");

        var patientExists = await _db.Patients.AnyAsync(p => p.Id == request.PatientId.Value, ct);
        if (!patientExists)
            return ResultDto<bool>.Fail("not_found", "Patient not found.");

        var visit = await _db.Visits.AsNoTracking().FirstOrDefaultAsync(v => v.Id == request.EncounterId.Value, ct);
        if (visit is null)
            return ResultDto<bool>.Fail("not_found", "Encounter/Visit not found.");

        if (visit.PatientId != request.PatientId.Value)
            return ResultDto<bool>.Fail("validation", "Encounter does not belong to patient.");

        entity.PatientId = request.PatientId.Value;
        entity.VisitId = request.EncounterId.Value;
        entity.Type = (GIPractice.Core.Enums.EndoscopyType)request.EndoscopyTypeId.Value;
        entity.PerformedAtUtc = request.StartUtc;
        entity.IsUrgent = request.IsUrgent;
        entity.Notes = request.Notes;

        await _db.SaveChangesAsync(ct);
        return ResultDto<bool>.Ok(true);
    }

    public async Task<ResultDto<bool>> DeleteAsync(EndoscopyId id, CancellationToken ct)
    {
        var entity = await _db.Endoscopies.FirstOrDefaultAsync(e => e.Id == id.Value, ct);
        if (entity is null)
            return ResultDto<bool>.Fail("not_found", "Endoscopy not found.");

        _db.Endoscopies.Remove(entity); // will soft-delete via your SaveChanges hook
        await _db.SaveChangesAsync(ct);

        return ResultDto<bool>.Ok(true);
    }

    private static IQueryable<GIPractice.Core.Entities.Endoscopy> ApplySort(
        IQueryable<GIPractice.Core.Entities.Endoscopy> q,
        SortDto? sort)
    {
        if (sort is null)
            return q.OrderByDescending(e => e.PerformedAtUtc).ThenByDescending(e => e.Id);

        var field = (sort.Field ?? "").Trim().ToLowerInvariant();
        var desc = sort.Desc;

        return field switch
        {
            "startutc" or "performedatutc" or "date" =>
                desc ? q.OrderByDescending(e => e.PerformedAtUtc) : q.OrderBy(e => e.PerformedAtUtc),

            "id" =>
                desc ? q.OrderByDescending(e => e.Id) : q.OrderBy(e => e.Id),

            "patientid" =>
                desc ? q.OrderByDescending(e => e.PatientId) : q.OrderBy(e => e.PatientId),

            "encounterid" or "visitid" =>
                desc ? q.OrderByDescending(e => e.VisitId) : q.OrderBy(e => e.VisitId),

            _ => q.OrderByDescending(e => e.PerformedAtUtc).ThenByDescending(e => e.Id)
        };
    }

    private static PagedResultDto<EndoscopyListItemDto> EmptyPage(EndoscopySearchRequestDto request)
        => new(Array.Empty<EndoscopyListItemDto>(), 0, request.Paging?.Page ?? 1, request.Paging?.PageSize ?? 50);

    private static byte[] MakeRowVersion(GIPractice.Core.Entities.Endoscopy e)
        => BitConverter.GetBytes((e.UpdatedAtUtc ?? e.CreatedAtUtc).Ticks);
}
