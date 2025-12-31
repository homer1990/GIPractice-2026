using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;

namespace GIPractice.Api.Pathology;

public sealed class InMemoryPathologyReportsStore(InMemoryPathologyRepository repo) : IPathologyReportsStore
{
    private static string MakePathologistRecordId(PathologistId pid, EndoscopyId eid)
        => $"{pid.Value}-{eid.Value}";

    public Task<ResultDto<PagedResultDto<PathologyReportListItemDto>>> SearchAsync(PathologyReportSearchRequestDto request, CancellationToken ct = default)
    {
        var snapshot = repo.SnapshotReports();
        IEnumerable<InMemoryPathologyRepository.ReportRow> q = snapshot;

        if (request.PatientId is not null) q = q.Where(x => x.PatientId == request.PatientId.Value);
        if (request.EndoscopyId is not null) q = q.Where(x => x.EndoscopyId == request.EndoscopyId.Value);
        if (request.PathologistId is not null) q = q.Where(x => x.PathologistId == request.PathologistId.Value);

        if (!string.IsNullOrWhiteSpace(request.DispatchParcelCode))
            q = q.Where(x => x.DispatchParcelCode == request.DispatchParcelCode);

        if (request.Status is not null) q = q.Where(x => x.Status == request.Status.Value);
        if (request.IsUrgent is not null) q = q.Where(x => x.IsUrgent == request.IsUrgent.Value);

        if (request.SentFromUtc is not null) q = q.Where(x => x.SentAtUtc >= request.SentFromUtc.Value);
        if (request.SentToUtc is not null) q = q.Where(x => x.SentAtUtc <= request.SentToUtc.Value);
        if (request.ReceivedFromUtc is not null) q = q.Where(x => x.ReceivedAtUtc >= request.ReceivedFromUtc.Value);
        if (request.ReceivedToUtc is not null) q = q.Where(x => x.ReceivedAtUtc <= request.ReceivedToUtc.Value);

        var paging = request.Paging ?? new PagedRequestDto(1, 50);
        var total = q.Count();

        var items = q
            .OrderByDescending(x => x.SentAtUtc ?? DateTime.MinValue)
            .ThenByDescending(x => x.Id.Value)
            .Skip((paging.Page - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .Select(x => new PathologyReportListItemDto(
                Id: x.Id,
                PatientId: x.PatientId,
                EndoscopyId: x.EndoscopyId,
                PathologistId: x.PathologistId,
                PathologistRecordId: MakePathologistRecordId(x.PathologistId, x.EndoscopyId),
                DispatchParcelCode: x.DispatchParcelCode,
                Status: x.Status,
                IsUrgent: x.IsUrgent,
                SentAtUtc: x.SentAtUtc,
                ReceivedAtUtc: x.ReceivedAtUtc,
                DocumentKind: x.DocumentKind == PathologyDocumentKind.Unknown ? null : x.DocumentKind,
                RowVersion: x.RowVersion))
            .ToList();

        return Task.FromResult(ResultDto<PagedResultDto<PathologyReportListItemDto>>.Ok(
            new PagedResultDto<PathologyReportListItemDto>(items, total, paging.Page, paging.PageSize)));
    }

    public Task<ResultDto<PathologyReportDto>> GetAsync(PathologyReportId id, CancellationToken ct = default)
    {
        if (!repo.TryGetReport(id, out var row))
            return Task.FromResult(ResultDto<PathologyReportDto>.Fail(new ErrorDto(ErrorCodes.NotFound, "Pathology report not found.")));

        return Task.FromResult(ResultDto<PathologyReportDto>.Ok(ToDto(row)));
    }

    public Task<ResultDto<PathologyReportId>> CreateAsync(PathologyReportUpsertRequestDto request, CancellationToken ct = default)
    {
        try
        {
            var id = repo.CreateReport(request, out _);
            return Task.FromResult(ResultDto<PathologyReportId>.Ok(id));
        }
        catch (InvalidOperationException ex)
        {
            return Task.FromResult(ResultDto<PathologyReportId>.Fail(new ErrorDto(ErrorCodes.Conflict, ex.Message)));
        }
    }

    public Task<ResultDto<bool>> UpdateAsync(PathologyReportUpsertRequestDto request, CancellationToken ct = default)
    {
        if (request.Id is null)
            return Task.FromResult(ResultDto<bool>.Fail(new ErrorDto(ErrorCodes.Validation, "Id is required for update.")));

        var id = request.Id.Value;

        lock (repo.Sync)
        {
            if (!repo.TryGetReport(id, out var row))
                return Task.FromResult(ResultDto<bool>.Fail(new ErrorDto(ErrorCodes.NotFound, "Pathology report not found.")));

            if (request.RowVersion is not null && !request.RowVersion.SequenceEqual(row.RowVersion))
                return Task.FromResult(ResultDto<bool>.Fail(new ErrorDto(ErrorCodes.Conflict, "RowVersion conflict.")));

            repo.UpdateReport(request, row);
            return Task.FromResult(ResultDto<bool>.Ok(true));
        }
    }

    public Task<ResultDto<bool>> DeleteAsync(PathologyReportId id, CancellationToken ct = default)
    {
        var ok = repo.DeleteReport(id);
        return Task.FromResult(ok
            ? ResultDto<bool>.Ok(true)
            : ResultDto<bool>.Fail(new ErrorDto(ErrorCodes.NotFound, "Pathology report not found.")));
    }

    private static PathologyReportDto ToDto(InMemoryPathologyRepository.ReportRow x) => new(
        Id: x.Id,
        PatientId: x.PatientId,
        EndoscopyId: x.EndoscopyId,
        PathologistId: x.PathologistId,
        PathologistRecordId: MakePathologistRecordId(x.PathologistId, x.EndoscopyId),
        DispatchParcelCode: x.DispatchParcelCode,
        SentAtUtc: x.SentAtUtc,
        ReceivedAtUtc: x.ReceivedAtUtc,
        Notes: x.Notes,
        ClinicalInfo: x.ClinicalInfo,
        MacroscopyText: x.MacroscopyText,
        DiagnosisText: x.DiagnosisText,
        Status: x.Status,
        IsUrgent: x.IsUrgent,
        DocumentFileId: x.DocumentFileId,
        DocumentKind: x.DocumentKind,
        DocumentFileName: x.DocumentFileName,
        DocumentContentType: x.DocumentContentType,
        Document: x.Document,
        RowVersion: x.RowVersion);
}
