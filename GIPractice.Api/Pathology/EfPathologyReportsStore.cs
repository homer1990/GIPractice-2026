using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;
using GIPractice.Core.Entities;
using GIPractice.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Pathology;

public sealed class EfPathologyReportsStore(AppDbContext db) : IPathologyReportsStore
{
    private readonly AppDbContext _db = db;

    private static string MakePathologistRecordId(int pathologistId, int endoscopyId)
        => $"{pathologistId}-{endoscopyId}";

    public async Task<ResultDto<PagedResultDto<PathologyReportListItemDto>>> SearchAsync(
        PathologyReportSearchRequestDto request,
        CancellationToken ct = default)
    {
        var paging = request.Paging ?? new PagedRequestDto(1, 50);

        if (paging.Page < 1)
            return ResultDto<PagedResultDto<PathologyReportListItemDto>>.Fail("validation", "Page must be >= 1.");

        if (paging.PageSize is < 1 or > 500)
            return ResultDto<PagedResultDto<PathologyReportListItemDto>>.Fail("validation", "PageSize must be between 1 and 500.");

        IQueryable<PathologyReport> q = _db.PathologyReports.AsNoTracking();

        if (request.PatientId is not null)
            q = q.Where(x => x.PatientId == request.PatientId.Value.Value);

        if (request.EndoscopyId is not null)
            q = q.Where(x => x.EndoscopyId == request.EndoscopyId.Value.Value);

        if (request.PathologistId is not null)
            q = q.Where(x => x.PathologistId == request.PathologistId.Value.Value);

        if (!string.IsNullOrWhiteSpace(request.DispatchParcelCode))
            q = q.Where(x => x.Parcel != null && x.Parcel.ParcelCode == request.DispatchParcelCode);

        if (request.Status is not null)
            q = q.Where(x => x.Status == (GIPractice.Core.Enums.PathologyReportStatus)request.Status.Value);

        if (request.IsUrgent is not null)
            q = q.Where(x => x.IsUrgent == request.IsUrgent.Value);

        if (request.SentFromUtc is not null)
            q = q.Where(x => x.SentAtUtc >= request.SentFromUtc.Value);

        if (request.SentToUtc is not null)
            q = q.Where(x => x.SentAtUtc <= request.SentToUtc.Value);

        if (request.ReceivedFromUtc is not null)
            q = q.Where(x => x.ReceivedAtUtc >= request.ReceivedFromUtc.Value);

        if (request.ReceivedToUtc is not null)
            q = q.Where(x => x.ReceivedAtUtc <= request.ReceivedToUtc.Value);

        var total = await q.CountAsync(ct);

        var rows = await q
            .OrderByDescending(x => x.SentAtUtc ?? DateTime.MinValue)
            .ThenByDescending(x => x.Id)
            .Skip((paging.Page - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .Select(x => new PathologyReportListItemDto(
                Id: new PathologyReportId(x.Id),
                PatientId: new PatientId(x.PatientId),
                EndoscopyId: new EndoscopyId(x.EndoscopyId),
                PathologistId: new PathologistId(x.PathologistId),
                PathologistRecordId: MakePathologistRecordId(x.PathologistId, x.EndoscopyId),
                DispatchParcelCode: x.Parcel != null ? x.Parcel.ParcelCode : null,
                Status: (PathologyReportStatus)x.Status,
                IsUrgent: x.IsUrgent,
                SentAtUtc: x.SentAtUtc,
                ReceivedAtUtc: x.ReceivedAtUtc,
                DocumentKind: x.DocumentKind == GIPractice.Core.Enums.PathologyDocumentKind.Unknown
                    ? null
                    : (PathologyDocumentKind?)x.DocumentKind,
                RowVersion: x.RowVersion))
            .ToListAsync(ct);

        return ResultDto<PagedResultDto<PathologyReportListItemDto>>.Ok(
            new PagedResultDto<PathologyReportListItemDto>(rows, total, paging.Page, paging.PageSize));
    }

    public async Task<ResultDto<PathologyReportDto>> GetAsync(PathologyReportId id, CancellationToken ct = default)
    {
        var x = await _db.PathologyReports
            .AsNoTracking()
            .Include(r => r.Parcel)
            .Include(r => r.DocumentFile)
            .FirstOrDefaultAsync(r => r.Id == id.Value, ct);

        if (x is null)
            return ResultDto<PathologyReportDto>.Fail("not_found", "Pathology report not found.");

        return ResultDto<PathologyReportDto>.Ok(ToDto(x));
    }

    public async Task<ResultDto<PathologyReportId>> CreateAsync(PathologyReportUpsertRequestDto request, CancellationToken ct = default)
    {
        if (request.Id is not null)
            return ResultDto<PathologyReportId>.Fail("invalid", "Id must be null when creating a pathology report.");

        // Option 1 contract: parcel membership is managed only by the Parcels API.
        if (request.DispatchParcelId is not null)
            return ResultDto<PathologyReportId>.Fail("validation", "DispatchParcelId cannot be set via Reports API. Use Parcels endpoints.");

        // Enforce uniqueness (PathologistId, EndoscopyId) pre-check
        var exists = await _db.PathologyReports
            .AnyAsync(r => r.PathologistId == request.PathologistId.Value && r.EndoscopyId == request.EndoscopyId.Value, ct);

        if (exists)
            return ResultDto<PathologyReportId>.Fail("conflict", "A pathology report already exists for this pathologist + endoscopy.");

        var entity = new PathologyReport
        {
            PatientId = request.PatientId.Value,
            EndoscopyId = request.EndoscopyId.Value,
            PathologistId = request.PathologistId.Value,

            SentAtUtc = request.SentAtUtc,
            ReceivedAtUtc = request.ReceivedAtUtc,

            Notes = TrimMaxNullable(request.Notes, 2000),
            ClinicalInfo = TrimMaxNullable(request.ClinicalInfo, 2000),
            MacroscopyText = TrimMaxNullable(request.MacroscopyText, 8000),
            DiagnosisText = TrimMaxNullable(request.DiagnosisText, 8000),

            Status = (GIPractice.Core.Enums.PathologyReportStatus)request.Status,
            IsUrgent = request.IsUrgent,

            DocumentFileId = request.DocumentFileId?.Value,
            DocumentKind = (GIPractice.Core.Enums.PathologyDocumentKind)request.DocumentKind
        };

        _db.PathologyReports.Add(entity);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            return ResultDto<PathologyReportId>.Fail("conflict", "Could not create pathology report (possible duplicate pathologist + endoscopy).");
        }

        return ResultDto<PathologyReportId>.Ok(new PathologyReportId(entity.Id));
    }

    public async Task<ResultDto<bool>> UpdateAsync(PathologyReportUpsertRequestDto request, CancellationToken ct = default)
    {
        if (request.Id is null)
            return ResultDto<bool>.Fail("invalid", "Id is required when updating a pathology report.");

        var entity = await _db.PathologyReports.FirstOrDefaultAsync(r => r.Id == request.Id.Value.Value, ct);
        if (entity is null)
            return ResultDto<bool>.Fail("not_found", "Pathology report not found.");

        if (request.RowVersion is not null && !request.RowVersion.SequenceEqual(entity.RowVersion))
            return ResultDto<bool>.Fail("conflict", "RowVersion conflict.");

        // Option 1 contract: IDs are immutable for existing reports.
        if (entity.PatientId != request.PatientId.Value)
            return ResultDto<bool>.Fail("validation", "PatientId is immutable.");
        if (entity.EndoscopyId != request.EndoscopyId.Value)
            return ResultDto<bool>.Fail("validation", "EndoscopyId is immutable.");
        if (entity.PathologistId != request.PathologistId.Value)
            return ResultDto<bool>.Fail("validation", "PathologistId is immutable.");

        // Parcel membership cannot be set/changed via Reports API.
        if (request.DispatchParcelId is not null)
            return ResultDto<bool>.Fail("validation", "DispatchParcelId cannot be set via Reports API. Use Parcels endpoints.");

        // SentAtUtc is controlled by parcel dispatch.
        if (request.SentAtUtc != entity.SentAtUtc)
            return ResultDto<bool>.Fail("validation", "SentAtUtc is controlled by parcel dispatch.");

        // ReceivedAtUtc invariants
        if (entity.ReceivedAtUtc is not null && request.ReceivedAtUtc is null)
            return ResultDto<bool>.Fail("validation", "ReceivedAtUtc cannot be cleared.");

        if (request.ReceivedAtUtc is not null)
        {
            if (entity.SentAtUtc is null)
                return ResultDto<bool>.Fail("validation", "Cannot set ReceivedAtUtc before SentAtUtc.");
            if (request.ReceivedAtUtc.Value < entity.SentAtUtc.Value)
                return ResultDto<bool>.Fail("validation", "ReceivedAtUtc cannot be earlier than SentAtUtc.");
        }

        // Content is only allowed after receipt.
        if (request.ReceivedAtUtc is null)
        {
            if (!string.IsNullOrWhiteSpace(request.MacroscopyText)
                || !string.IsNullOrWhiteSpace(request.DiagnosisText)
                || request.DocumentFileId is not null
                || request.DocumentKind != PathologyDocumentKind.Unknown)
            {
                return ResultDto<bool>.Fail("validation", "Report content can only be set after receipt.");
            }
        }

        // Status invariants
        if (request.Status == PathologyReportStatus.Dispatched && entity.SentAtUtc is null)
            return ResultDto<bool>.Fail("validation", "Cannot set status Dispatched without SentAtUtc.");

        if (request.Status == PathologyReportStatus.Received && request.ReceivedAtUtc is null)
            return ResultDto<bool>.Fail("validation", "Cannot set status Received without ReceivedAtUtc.");

        if (request.Status == PathologyReportStatus.Completed && request.ReceivedAtUtc is null)
            return ResultDto<bool>.Fail("validation", "Cannot set status Completed without ReceivedAtUtc.");

        entity.ReceivedAtUtc = request.ReceivedAtUtc;

        entity.Notes = TrimMaxNullable(request.Notes, 2000);
        entity.ClinicalInfo = TrimMaxNullable(request.ClinicalInfo, 2000);
        entity.MacroscopyText = TrimMaxNullable(request.MacroscopyText, 8000);
        entity.DiagnosisText = TrimMaxNullable(request.DiagnosisText, 8000);

        entity.Status = (GIPractice.Core.Enums.PathologyReportStatus)request.Status;
        entity.IsUrgent = request.IsUrgent;

        entity.DocumentFileId = request.DocumentFileId?.Value;
        entity.DocumentKind = (GIPractice.Core.Enums.PathologyDocumentKind)request.DocumentKind;

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            return ResultDto<bool>.Fail("conflict", "Could not update pathology report.");
        }

        return ResultDto<bool>.Ok(true);
    }

    public async Task<ResultDto<bool>> DeleteAsync(PathologyReportId id, CancellationToken ct = default)
    {
        var entity = await _db.PathologyReports.FirstOrDefaultAsync(r => r.Id == id.Value, ct);
        if (entity is null)
            return ResultDto<bool>.Fail("not_found", "Pathology report not found.");

        // If it's in a dispatched parcel, do not allow delete (chain-of-custody)
        if (entity.PathologyParcelId is not null)
        {
            var parcel = await _db.PathologyParcels.AsNoTracking().FirstOrDefaultAsync(p => p.Id == entity.PathologyParcelId.Value, ct);
            if (parcel?.DispatchedAtUtc is not null)
                return ResultDto<bool>.Fail("validation", "Cannot delete a report that belongs to a dispatched parcel.");
        }

        _db.PathologyReports.Remove(entity);
        await _db.SaveChangesAsync(ct);

        return ResultDto<bool>.Ok(true);
    }

    private static PathologyReportDto ToDto(PathologyReport x) => new(
        Id: new PathologyReportId(x.Id),
        PatientId: new PatientId(x.PatientId),
        EndoscopyId: new EndoscopyId(x.EndoscopyId),
        PathologistId: new PathologistId(x.PathologistId),
        PathologistRecordId: MakePathologistRecordId(x.PathologistId, x.EndoscopyId),
        DispatchParcelCode: x.Parcel?.ParcelCode,
        SentAtUtc: x.SentAtUtc,
        ReceivedAtUtc: x.ReceivedAtUtc,
        Notes: x.Notes,
        ClinicalInfo: x.ClinicalInfo,
        MacroscopyText: x.MacroscopyText,
        DiagnosisText: x.DiagnosisText,
        Status: (PathologyReportStatus)x.Status,
        IsUrgent: x.IsUrgent,
        DocumentFileId: x.DocumentFileId is null ? null : new MediaFileId(x.DocumentFileId.Value),
        DocumentKind: (PathologyDocumentKind)x.DocumentKind,
        DocumentFileName: x.DocumentFile?.FileName,
        DocumentContentType: x.DocumentFile?.ContentType,
        Document: null,
        RowVersion: x.RowVersion);

    private static string? TrimMaxNullable(string? s, int max)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        s = s.Trim();
        return s.Length <= max ? s : s[..max];
    }
}
