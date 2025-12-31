using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;

namespace GIPractice.Api.Pathology;

public sealed class InMemoryPathologyParcelsStore(InMemoryPathologyRepository repo) : IPathologyParcelsStore
{
    public Task<ResultDto<PagedResultDto<PathologyParcelDto>>> SearchAsync(PathologyParcelSearchRequestDto request, CancellationToken ct = default)
    {
        var parcels = repo.SnapshotParcels();
        var reports = repo.SnapshotReports();

        IEnumerable<InMemoryPathologyRepository.ParcelRow> q = parcels;

        if (request.PathologistId is not null)
            q = q.Where(x => x.PathologistId == request.PathologistId.Value);

        if (request.CreatedFromUtc is not null)
            q = q.Where(x => x.CreatedAtUtc >= request.CreatedFromUtc.Value);

        if (request.CreatedToUtc is not null)
            q = q.Where(x => x.CreatedAtUtc <= request.CreatedToUtc.Value);

        // HasUrgent is computed from assigned reports
        if (request.HasUrgent is not null)
        {
            q = q.Where(p =>
            {
                var urgent = reports.Any(r => r.PathologistId == p.PathologistId &&
                                             r.DispatchParcelCode == p.ParcelCode &&
                                             r.IsUrgent);
                return urgent == request.HasUrgent.Value;
            });
        }

        var paging = request.Paging ?? new PagedRequestDto(1, 50);
        var total = q.Count();

        var items = q
            .OrderByDescending(x => x.CreatedAtUtc)
            .ThenByDescending(x => x.Id.Value)
            .Skip((paging.Page - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .Select(p => ToDto(p, reports))
            .ToList();

        return Task.FromResult(ResultDto<PagedResultDto<PathologyParcelDto>>.Ok(
            new PagedResultDto<PathologyParcelDto>(items, total, paging.Page, paging.PageSize)));
    }

    public Task<ResultDto<PathologyParcelDto>> GetByKeyAsync(PathologyParcelKeyDto key, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(key.ParcelCode))
            return Task.FromResult(ResultDto<PathologyParcelDto>.Fail(new ErrorDto(ErrorCodes.Validation, "ParcelCode is required.")));

        if (!repo.TryGetParcelByKey(key.PathologistId, key.ParcelCode, out var parcel))
            return Task.FromResult(ResultDto<PathologyParcelDto>.Fail(new ErrorDto(ErrorCodes.NotFound, "Parcel not found.")));

        var reports = repo.SnapshotReports();
        return Task.FromResult(ResultDto<PathologyParcelDto>.Ok(ToDto(parcel, reports)));
    }

    public Task<ResultDto<PathologyParcelId>> CreateAsync(PathologyParcelCreateRequestDto request, CancellationToken ct = default)
    {
        var id = repo.CreateParcel(request, out _);
        return Task.FromResult(ResultDto<PathologyParcelId>.Ok(id));
    }

    public Task<ResultDto<bool>> UpdateByKeyAsync(PathologyParcelKeyDto key, PathologyParcelUpdateRequestDto request, CancellationToken ct = default)
    {
        if (!repo.TryGetParcelByKey(key.PathologistId, key.ParcelCode, out var parcel))
            return Task.FromResult(ResultDto<bool>.Fail(new ErrorDto(ErrorCodes.NotFound, "Parcel not found.")));

        lock (repo.Sync)
        {
            // RowVersion guard if provided
            if (request.RowVersion is not null && !request.RowVersion.SequenceEqual(parcel.RowVersion))
                return Task.FromResult(ResultDto<bool>.Fail(new ErrorDto(ErrorCodes.Conflict, "RowVersion conflict.")));

            parcel.DispatchedAtUtc = request.DispatchedAtUtc;
            parcel.CourierName = request.CourierName;
            parcel.TrackingNumber = request.TrackingNumber;
            parcel.Notes = request.Notes;
            parcel.RowVersion = repo.NextRv();

            return Task.FromResult(ResultDto<bool>.Ok(true));
        }
    }

    public Task<ResultDto<bool>> AssignReportsAsync(PathologyParcelKeyDto key, PathologyParcelAssignReportsRequestDto request, CancellationToken ct = default)
    {
        if (!repo.TryGetParcelByKey(key.PathologistId, key.ParcelCode, out var parcel))
            return Task.FromResult(ResultDto<bool>.Fail(new ErrorDto(ErrorCodes.NotFound, "Parcel not found.")));

        lock (repo.Sync)
        {
            try
            {
                repo.AssignReportsToParcel(parcel, request.ReportIds);
                return Task.FromResult(ResultDto<bool>.Ok(true));
            }
            catch (KeyNotFoundException ex)
            {
                return Task.FromResult(ResultDto<bool>.Fail(new ErrorDto(ErrorCodes.NotFound, ex.Message)));
            }
            catch (InvalidOperationException ex)
            {
                return Task.FromResult(ResultDto<bool>.Fail(new ErrorDto(ErrorCodes.Validation, ex.Message)));
            }
        }
    }

    public Task<ResultDto<bool>> UnassignReportsAsync(PathologyParcelKeyDto key, PathologyParcelAssignReportsRequestDto request, CancellationToken ct = default)
    {
        if (!repo.TryGetParcelByKey(key.PathologistId, key.ParcelCode, out var parcel))
            return Task.FromResult(ResultDto<bool>.Fail(new ErrorDto(ErrorCodes.NotFound, "Parcel not found.")));

        lock (repo.Sync)
        {
            repo.UnassignReportsFromParcel(parcel, request.ReportIds);
            return Task.FromResult(ResultDto<bool>.Ok(true));
        }
    }

    private static PathologyParcelDto ToDto(InMemoryPathologyRepository.ParcelRow p, InMemoryPathologyRepository.ReportRow[] reports)
    {
        var assigned = reports.Where(r => r.PathologistId == p.PathologistId && r.DispatchParcelCode == p.ParcelCode).ToArray();
        var count = assigned.Length;
        var urgent = assigned.Any(r => r.IsUrgent);
        
        return new PathologyParcelDto(
            Id: p.Id,
            PathologistId: p.PathologistId,
            ParcelCode: p.ParcelCode,
            CreatedAtUtc: p.CreatedAtUtc,
            DispatchedAtUtc: p.DispatchedAtUtc,
            CourierName: p.CourierName,
            TrackingNumber: p.TrackingNumber,
            Notes: p.Notes,
            ReportsCount: count,
            HasUrgent: urgent,
            RowVersion: p.RowVersion
            );
    }
}
