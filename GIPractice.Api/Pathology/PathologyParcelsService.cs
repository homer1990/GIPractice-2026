using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;

namespace GIPractice.Api.Pathology;

public sealed class PathologyParcelsService(
    IPathologyParcelsStore store,
    InMemoryPathologyRepository repo) : IPathologyParcelsService
{
    public Task<ResultDto<PagedResultDto<PathologyParcelDto>>> SearchAsync(
        PathologyParcelSearchRequestDto request,
        CancellationToken cancellationToken = default)
        => store.SearchAsync(request, cancellationToken);

    // ✅ required by Contracts: GetAsync(PathologyParcelId, ...)
    public Task<ResultDto<PathologyParcelDto>> GetAsync(
        PathologyParcelId id,
        CancellationToken cancellationToken = default)
    {
        if (!repo.TryGetParcelById(id, out var parcel))
            return Task.FromResult(ResultDto<PathologyParcelDto>.Fail(
                new ErrorDto(ErrorCodes.NotFound, "Parcel not found.")));

        // Reuse store logic via key (keeps a single mapping path)
        var key = new PathologyParcelKeyDto(parcel.PathologistId, parcel.ParcelCode);
        return store.GetByKeyAsync(key, cancellationToken);
    }

    public Task<ResultDto<PathologyParcelDto>> GetByKeyAsync(PathologyParcelKeyDto key, CancellationToken cancellationToken = default)
    => store.GetByKeyAsync(key, cancellationToken);

    public Task<ResultDto<PathologyParcelId>> CreateAsync(PathologyParcelCreateRequestDto request, CancellationToken cancellationToken = default)
        => store.CreateAsync(request, cancellationToken);

    // ✅ your requested addition: UpdateByKeyAsync
    public Task<ResultDto<bool>> UpdateByKeyAsync(
        PathologyParcelKeyDto key,
        PathologyParcelUpdateRequestDto request,
        CancellationToken cancellationToken = default)
        => store.UpdateByKeyAsync(key, request, cancellationToken);

    // Keep the original UpdateAsync if your contracts still have it.
    // If UpdateAsync exists in IPathologyParcelsService, implement it here.
    public Task<ResultDto<bool>> UpdateAsync(
        PathologyParcelUpdateRequestDto request,
        CancellationToken cancellationToken = default)
        => Task.FromResult(ResultDto<bool>.Fail(
            new ErrorDto(ErrorCodes.Validation, "Use UpdateByKeyAsync (keyed route) endpoint.")));

    public Task<ResultDto<bool>> AssignReportsAsync(
        PathologyParcelKeyDto key,
        PathologyParcelAssignReportsRequestDto request,
        CancellationToken cancellationToken = default)
        => store.AssignReportsAsync(key, request, cancellationToken);

    public Task<ResultDto<bool>> UnassignReportsAsync(
        PathologyParcelKeyDto key,
        PathologyParcelAssignReportsRequestDto request,
        CancellationToken cancellationToken = default)
        => store.UnassignReportsAsync(key, request, cancellationToken);
}
