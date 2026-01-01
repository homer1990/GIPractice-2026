using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;
using GIPractice.Core.Entities;
using GIPractice.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Pathology;

public sealed class EfPathologyParcelsStore(AppDbContext db) : IPathologyParcelsStore
{
    private readonly AppDbContext _db = db;

    public async Task<ResultDto<PagedResultDto<PathologyParcelDto>>> SearchAsync(
        PathologyParcelSearchRequestDto request,
        CancellationToken ct = default)
    {
        var paging = request.Paging ?? new PagedRequestDto(1, 50);

        if (paging.Page < 1)
            return ResultDto<PagedResultDto<PathologyParcelDto>>.Fail("validation", "Page must be >= 1.");

        if (paging.PageSize is < 1 or > 500)
            return ResultDto<PagedResultDto<PathologyParcelDto>>.Fail("validation", "PageSize must be between 1 and 500.");

        IQueryable<PathologyParcel> q = _db.PathologyParcels.AsNoTracking();

        if (request.PathologistId is not null)
            q = q.Where(p => p.PathologistId == request.PathologistId.Value.Value);

        if (request.CreatedFromUtc is not null)
            q = q.Where(p => p.CreatedAtUtc >= request.CreatedFromUtc.Value);

        if (request.CreatedToUtc is not null)
            q = q.Where(p => p.CreatedAtUtc <= request.CreatedToUtc.Value);

        if (request.HasUrgent is not null)
        {
            var wantUrgent = request.HasUrgent.Value;
            q = q.Where(p => p.Reports.Any(r => r.IsUrgent) == wantUrgent);
        }

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(p => p.CreatedAtUtc)
            .ThenByDescending(p => p.Id)
            .Skip((paging.Page - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .Select(p => new PathologyParcelDto(
                Id: new PathologyParcelId(p.Id),
                PathologistId: new PathologistId(p.PathologistId),
                ParcelCode: p.ParcelCode,
                CreatedAtUtc: p.CreatedAtUtc,
                DispatchedAtUtc: p.DispatchedAtUtc,
                CourierName: p.CourierName,
                TrackingNumber: p.TrackingNumber,
                Notes: p.Notes,
                MonetarySum: p.MonetarySum,
                ReportsCount: p.Reports.Count,
                HasUrgent: p.Reports.Any(r => r.IsUrgent),
                RowVersion: p.RowVersion))
            .ToListAsync(ct);

        return ResultDto<PagedResultDto<PathologyParcelDto>>.Ok(
            new PagedResultDto<PathologyParcelDto>(items, total, paging.Page, paging.PageSize));
    }

    public async Task<ResultDto<PathologyParcelDto>> GetByKeyAsync(PathologyParcelKeyDto key, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key.ParcelCode))
            return ResultDto<PathologyParcelDto>.Fail("validation", "ParcelCode is required.");

        var p = await _db.PathologyParcels
            .AsNoTracking()
            .Where(x => x.PathologistId == key.PathologistId.Value && x.ParcelCode == key.ParcelCode)
            .Select(x => new PathologyParcelDto(
                Id: new PathologyParcelId(x.Id),
                PathologistId: new PathologistId(x.PathologistId),
                ParcelCode: x.ParcelCode,
                CreatedAtUtc: x.CreatedAtUtc,
                DispatchedAtUtc: x.DispatchedAtUtc,
                CourierName: x.CourierName,
                TrackingNumber: x.TrackingNumber,
                Notes: x.Notes,
                MonetarySum: x.MonetarySum,
                ReportsCount: x.Reports.Count,
                HasUrgent: x.Reports.Any(r => r.IsUrgent),
                RowVersion: x.RowVersion))
            .FirstOrDefaultAsync(ct);

        return p is null
            ? ResultDto<PathologyParcelDto>.Fail("not_found", "Parcel not found.")
            : ResultDto<PathologyParcelDto>.Ok(p);
    }

    public async Task<ResultDto<PathologyParcelDto>> GetAsync(PathologyParcelId id, CancellationToken ct = default)
    {
        var p = await _db.PathologyParcels
            .AsNoTracking()
            .Where(x => x.Id == id.Value)
            .Select(x => new PathologyParcelDto(
                Id: new PathologyParcelId(x.Id),
                PathologistId: new PathologistId(x.PathologistId),
                ParcelCode: x.ParcelCode,
                CreatedAtUtc: x.CreatedAtUtc,
                DispatchedAtUtc: x.DispatchedAtUtc,
                CourierName: x.CourierName,
                TrackingNumber: x.TrackingNumber,
                Notes: x.Notes,
                MonetarySum: x.MonetarySum,
                ReportsCount: x.Reports.Count,
                HasUrgent: x.Reports.Any(r => r.IsUrgent),
                RowVersion: x.RowVersion))
            .FirstOrDefaultAsync(ct);

        return p is null
            ? ResultDto<PathologyParcelDto>.Fail("not_found", "Parcel not found.")
            : ResultDto<PathologyParcelDto>.Ok(p);
    }

    public async Task<ResultDto<PathologyParcelId>> CreateAsync(PathologyParcelCreateRequestDto request, CancellationToken ct = default)
    {
        if (request.EndoscopyIds is null || request.EndoscopyIds.Length == 0)
            return ResultDto<PathologyParcelId>.Fail("validation", "EndoscopyIds is required.");

        var pathologistExists = await _db.Pathologists.AnyAsync(p => p.Id == request.PathologistId.Value, ct);
        if (!pathologistExists)
            return ResultDto<PathologyParcelId>.Fail("not_found", "Pathologist not found.");

        var parcelCode = await GenerateParcelCodeAsync(request.PathologistId.Value, ct);

        var parcel = new PathologyParcel
        {
            PathologistId = request.PathologistId.Value,
            ParcelCode = parcelCode,
            DispatchedAtUtc = request.DispatchedAtUtc,
            CourierName = TrimMaxNullable(request.CourierName, 100),
            TrackingNumber = TrimMaxNullable(request.TrackingNumber, 100),
            Notes = TrimMaxNullable(request.Notes, 500),
        };

        _db.PathologyParcels.Add(parcel);

        try
        {
            var ids = request.EndoscopyIds.Select(x => x.Value).Distinct().ToArray();

            await AttachEndoscopiesToParcelAsync(
                parcel,
                ids,
                disallowIfAlreadyInAnotherParcel: true,
                ct);

            parcel.MonetarySum = await ComputeMonetarySumForEndoscopiesAsync(ids, ct);

            await _db.SaveChangesAsync(ct);
        }
        catch (InvalidOperationException ex)
        {
            return ResultDto<PathologyParcelId>.Fail("validation", ex.Message);
        }

        return ResultDto<PathologyParcelId>.Ok(new PathologyParcelId(parcel.Id));
    }

    public async Task<ResultDto<bool>> UpdateByKeyAsync(PathologyParcelKeyDto key, PathologyParcelUpdateRequestDto request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key.ParcelCode))
            return ResultDto<bool>.Fail("validation", "ParcelCode is required.");

        var parcel = await _db.PathologyParcels
            .Include(p => p.Reports)
            .FirstOrDefaultAsync(p => p.PathologistId == key.PathologistId.Value && p.ParcelCode == key.ParcelCode, ct);

        if (parcel is null)
            return ResultDto<bool>.Fail("not_found", "Parcel not found.");

        if (request.Id.Value != parcel.Id)
            return ResultDto<bool>.Fail("validation", "Request Id does not match the parcel referenced by the URL key.");

        if (request.RowVersion is not null && !request.RowVersion.SequenceEqual(parcel.RowVersion))
            return ResultDto<bool>.Fail("conflict", "RowVersion conflict.");

        // DispatchedAt can only transition null -> value (locks items)
        if (parcel.DispatchedAtUtc is not null && request.DispatchedAtUtc is null)
            return ResultDto<bool>.Fail("validation", "Cannot clear DispatchedAtUtc once set.");

        var dispatchTransition = parcel.DispatchedAtUtc is null && request.DispatchedAtUtc is not null;

        parcel.DispatchedAtUtc = request.DispatchedAtUtc;
        parcel.CourierName = TrimMaxNullable(request.CourierName, 100);
        parcel.TrackingNumber = TrimMaxNullable(request.TrackingNumber, 100);
        parcel.Notes = TrimMaxNullable(request.Notes, 500);

        if (dispatchTransition)
        {
            foreach (var r in parcel.Reports)
            {
                r.SentAtUtc ??= request.DispatchedAtUtc;

                if (r.Status == GIPractice.Core.Enums.PathologyReportStatus.Draft)
                    r.Status = GIPractice.Core.Enums.PathologyReportStatus.Dispatched;
            }
        }

        // Recalculate monetary sum (refreshes if endoscopy costs changed).
        parcel.MonetarySum = await ComputeMonetarySumForEndoscopiesAsync(
            [.. parcel.Reports.Select(r => r.EndoscopyId).Distinct()],
            ct);

        await _db.SaveChangesAsync(ct);
        return ResultDto<bool>.Ok(true);
    }

    public async Task<ResultDto<bool>> AssignEndoscopiesAsync(PathologyParcelKeyDto key, PathologyParcelAssignEndoscopiesRequestDto request, CancellationToken ct = default)
    {
        if (request.EndoscopyIds is null || request.EndoscopyIds.Length == 0)
            return ResultDto<bool>.Fail("validation", "EndoscopyIds is required.");

        var parcel = await _db.PathologyParcels
            .FirstOrDefaultAsync(p => p.PathologistId == key.PathologistId.Value && p.ParcelCode == key.ParcelCode, ct);

        if (parcel is null)
            return ResultDto<bool>.Fail("not_found", "Parcel not found.");

        if (parcel.DispatchedAtUtc is not null)
            return ResultDto<bool>.Fail("validation", "Cannot modify a dispatched parcel.");

        try
        {
            var ids = request.EndoscopyIds.Select(x => x.Value).Distinct().ToArray();

            await AttachEndoscopiesToParcelAsync(
                parcel,
                ids,
                disallowIfAlreadyInAnotherParcel: true,
                ct);

            // DB state doesn't include unsaved tracked reports yet, so compute using (current in DB) + (requested ids)
            var current = await _db.PathologyReports.AsNoTracking()
                .Where(r => r.PathologyParcelId == parcel.Id)
                .Select(r => r.EndoscopyId)
                .ToListAsync(ct);

            var all = current.Concat(ids).Distinct().ToArray();
            parcel.MonetarySum = await ComputeMonetarySumForEndoscopiesAsync(all, ct);

            await _db.SaveChangesAsync(ct);
        }
        catch (InvalidOperationException ex)
        {
            return ResultDto<bool>.Fail("validation", ex.Message);
        }

        return ResultDto<bool>.Ok(true);
    }

    public async Task<ResultDto<bool>> UnassignEndoscopiesAsync(PathologyParcelKeyDto key, PathologyParcelAssignEndoscopiesRequestDto request, CancellationToken ct = default)
    {
        if (request.EndoscopyIds is null || request.EndoscopyIds.Length == 0)
            return ResultDto<bool>.Fail("validation", "EndoscopyIds is required.");

        var parcel = await _db.PathologyParcels
            .FirstOrDefaultAsync(p => p.PathologistId == key.PathologistId.Value && p.ParcelCode == key.ParcelCode, ct);

        if (parcel is null)
            return ResultDto<bool>.Fail("not_found", "Parcel not found.");

        if (parcel.DispatchedAtUtc is not null)
            return ResultDto<bool>.Fail("validation", "Cannot modify a dispatched parcel.");

        var ids = request.EndoscopyIds.Select(x => x.Value).Distinct().ToArray();

        var reports = await _db.PathologyReports
            .Where(r => r.PathologyParcelId == parcel.Id && r.PathologistId == parcel.PathologistId && ids.Contains(r.EndoscopyId))
            .ToListAsync(ct);

        // Chain-of-custody guard: draft-only
        if (reports.Any(r => r.Status != GIPractice.Core.Enums.PathologyReportStatus.Draft || r.SentAtUtc is not null || r.ReceivedAtUtc is not null))
            return ResultDto<bool>.Fail("validation", "Cannot unassign endoscopies from a parcel once any linked report has progressed beyond Draft.");

        foreach (var r in reports)
        {
            r.PathologyParcelId = null;
            r.Parcel = null;
        }

        // Recalculate monetary sum (refreshes if endoscopy costs changed).
        var current = await _db.PathologyReports.AsNoTracking()
            .Where(r => r.PathologyParcelId == parcel.Id)
            .Select(r => r.EndoscopyId)
            .ToListAsync(ct);

        var remaining = current.Except(ids).Distinct().ToArray();
        parcel.MonetarySum = await ComputeMonetarySumForEndoscopiesAsync(remaining, ct);

        await _db.SaveChangesAsync(ct);
        return ResultDto<bool>.Ok(true);
    }

    private async Task AttachEndoscopiesToParcelAsync(
        PathologyParcel parcel,
        int[] endoscopyIds,
        bool disallowIfAlreadyInAnotherParcel,
        CancellationToken ct)
    {
        var ids = endoscopyIds.Distinct().ToArray();

        // Validate endoscopies exist
        var endos = await _db.Endoscopies
            .AsNoTracking()
            .Where(e => ids.Contains(e.Id))
            .Select(e => new { e.Id, e.PatientId, e.IsUrgent })
            .ToListAsync(ct);

        var missing = ids.Except(endos.Select(e => e.Id)).ToArray();
        if (missing.Length > 0)
            throw new InvalidOperationException($"Endoscopy not found: {string.Join(", ", missing)}");

        // Validate biopsy bottles exist
        var withBottles = await _db.BiopsyBottles
            .AsNoTracking()
            .Where(b => ids.Contains(b.EndoscopyId))
            .Select(b => b.EndoscopyId)
            .Distinct()
            .ToListAsync(ct);

        var withoutBottles = ids.Except(withBottles).ToArray();
        if (withoutBottles.Length > 0)
            throw new InvalidOperationException($"Endoscopy has no biopsy bottles (cannot be dispatched): {string.Join(", ", withoutBottles)}");

        // Load existing reports for (pathologist, endoscopy)
        var existing = await _db.PathologyReports
            .Include(r => r.Parcel)
            .Where(r => r.PathologistId == parcel.PathologistId && ids.Contains(r.EndoscopyId))
            .ToListAsync(ct);

        var existingByEndo = existing.ToDictionary(r => r.EndoscopyId, r => r);

        foreach (var e in endos)
        {
            if (existingByEndo.TryGetValue(e.Id, out var r))
            {
                if (disallowIfAlreadyInAnotherParcel && r.PathologyParcelId is not null && r.PathologyParcelId != parcel.Id)
                {
                    var otherCode = r.Parcel?.ParcelCode ?? r.PathologyParcelId.Value.ToString();
                    throw new InvalidOperationException($"Endoscopy {e.Id} is already assigned to another parcel ({otherCode}).");
                }

                r.PatientId = e.PatientId;
                r.IsUrgent = e.IsUrgent;
                r.PathologyParcelId = parcel.Id;
                r.Parcel = parcel;

                if (parcel.DispatchedAtUtc is not null)
                {
                    r.SentAtUtc ??= parcel.DispatchedAtUtc;
                    if (r.Status == GIPractice.Core.Enums.PathologyReportStatus.Draft)
                        r.Status = GIPractice.Core.Enums.PathologyReportStatus.Dispatched;
                }

                continue;
            }

            var nr = new PathologyReport
            {
                PatientId = e.PatientId,
                EndoscopyId = e.Id,
                PathologistId = parcel.PathologistId,
                PathologyParcelId = parcel.Id,
                Parcel = parcel,
                IsUrgent = e.IsUrgent,
                Status = parcel.DispatchedAtUtc is null
                    ? GIPractice.Core.Enums.PathologyReportStatus.Draft
                    : GIPractice.Core.Enums.PathologyReportStatus.Dispatched,
                SentAtUtc = parcel.DispatchedAtUtc,
                DocumentKind = GIPractice.Core.Enums.PathologyDocumentKind.Unknown
            };

            _db.PathologyReports.Add(nr);
        }
    }

    private async Task<decimal> ComputeMonetarySumForEndoscopiesAsync(int[] endoscopyIds, CancellationToken ct)
    {
        if (endoscopyIds is null || endoscopyIds.Length == 0)
            return 0m;

        // Sum biopsies cost across all endoscopies in the parcel.
        // Null cost is treated as 0 (until we add a proper pricing workflow).
        return await _db.Endoscopies
            .AsNoTracking()
            .Where(e => endoscopyIds.Contains(e.Id))
            .Select(e => e.BiopsiesCost ?? 0m)
            .SumAsync(ct);
    }

    private async Task<string> GenerateParcelCodeAsync(int pathologistId, CancellationToken ct)
    {
        var baseCode = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        var code = baseCode;
        var n = 1;

        while (await _db.PathologyParcels.AnyAsync(p => p.PathologistId == pathologistId && p.ParcelCode == code, ct))
        {
            code = $"{baseCode}-{n:00}";
            n++;
        }

        return code;
    }

    private static string? TrimMaxNullable(string? s, int max)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        s = s.Trim();
        return s.Length <= max ? s : s[..max];
    }
}
