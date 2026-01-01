using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public interface IPathologyParcelsService
{
    Task<ResultDto<PagedResultDto<PathologyParcelDto>>> SearchAsync(
        PathologyParcelSearchRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<PathologyParcelDto>> GetByKeyAsync(
        PathologyParcelKeyDto key,
        CancellationToken cancellationToken = default);

    Task<ResultDto<PathologyParcelDto>> GetAsync(
        PathologyParcelId id,
        CancellationToken cancellationToken = default);

    Task<ResultDto<PathologyParcelId>> CreateAsync(
        PathologyParcelCreateRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> UpdateAsync(
        PathologyParcelUpdateRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> AssignEndoscopiesAsync(
        PathologyParcelKeyDto key,
        PathologyParcelAssignEndoscopiesRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> UnassignEndoscopiesAsync(
        PathologyParcelKeyDto key,
        PathologyParcelAssignEndoscopiesRequestDto request,
        CancellationToken cancellationToken = default);
}
