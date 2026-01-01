using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;

namespace GIPractice.Api.Pathology;

public sealed class PathologyParcelsService(IPathologyParcelsStore store) : IPathologyParcelsService
{
    public Task<ResultDto<PagedResultDto<PathologyParcelDto>>> SearchAsync(
        PathologyParcelSearchRequestDto request,
        CancellationToken cancellationToken = default)
        => store.SearchAsync(request, cancellationToken);

    public Task<ResultDto<PathologyParcelDto>> GetByKeyAsync(
        PathologyParcelKeyDto key,
        CancellationToken cancellationToken = default)
        => store.GetByKeyAsync(key, cancellationToken);

    public Task<ResultDto<PathologyParcelDto>> GetAsync(
        PathologyParcelId id,
        CancellationToken cancellationToken = default)
        => store.GetAsync(id, cancellationToken);

    public Task<ResultDto<PathologyParcelId>> CreateAsync(
        PathologyParcelCreateRequestDto request,
        CancellationToken cancellationToken = default)
        => store.CreateAsync(request, cancellationToken);

    public async Task<ResultDto<bool>> UpdateAsync(
        PathologyParcelUpdateRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var current = await store.GetAsync(request.Id, cancellationToken);

        if (!current.IsSuccess)
            return ResultDto<bool>.Fail(current.Error ?? new ErrorDto(ErrorCodes.Unexpected, "Unknown error."));

        if (current.Value is null)
            return ResultDto<bool>.Fail(ErrorCodes.NotFound, "Parcel not found.");

        var key = new PathologyParcelKeyDto(current.Value.PathologistId, current.Value.ParcelCode);
        return await store.UpdateByKeyAsync(key, request, cancellationToken);
    }

    public Task<ResultDto<bool>> AssignEndoscopiesAsync(
        PathologyParcelKeyDto key,
        PathologyParcelAssignEndoscopiesRequestDto request,
        CancellationToken cancellationToken = default)
        => store.AssignEndoscopiesAsync(key, request, cancellationToken);

    public Task<ResultDto<bool>> UnassignEndoscopiesAsync(
        PathologyParcelKeyDto key,
        PathologyParcelAssignEndoscopiesRequestDto request,
        CancellationToken cancellationToken = default)
        => store.UnassignEndoscopiesAsync(key, request, cancellationToken);
}
