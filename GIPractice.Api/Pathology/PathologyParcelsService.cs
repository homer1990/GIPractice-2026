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

    // Older Contracts expect this
    public Task<ResultDto<PathologyParcelDto>> GetAsync(
        PathologyParcelId id,
        CancellationToken cancellationToken = default)
    {
        if (!repo.TryGetParcelById(id, out var parcel))
            return Task.FromResult(ResultDto<PathologyParcelDto>.Fail(
                new ErrorDto(ErrorCodes.NotFound, "Parcel not found.")));

        return store.GetByKeyAsync(
            new PathologyParcelKeyDto(parcel.PathologistId, parcel.ParcelCode),
            cancellationToken);
    }

    // Older Contracts expect this
    public Task<ResultDto<PathologyParcelDto>> GetByKeyAsync(
        PathologyParcelKeyDto key,
        CancellationToken cancellationToken = default)
        => store.GetByKeyAsync(key, cancellationToken);

    // Older Contracts expect this
    public Task<ResultDto<PathologyParcelId>> CreateAsync(
        PathologyParcelCreateRequestDto request,
        CancellationToken cancellationToken = default)
        => store.CreateAsync(request, cancellationToken);

    // Newer keyed update (your preference)
    public Task<ResultDto<bool>> UpdateByKeyAsync(
        PathologyParcelKeyDto key,
        PathologyParcelUpdateRequestDto request,
        CancellationToken cancellationToken = default)
        => store.UpdateByKeyAsync(key, request, cancellationToken);

    // If your Contracts *still* have UpdateAsync (non-keyed), keep it as a guard.
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
