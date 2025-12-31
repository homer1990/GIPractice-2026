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

    Task<ResultDto<bool>> AssignReportsAsync(
        PathologyParcelKeyDto key,
        PathologyParcelAssignReportsRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> UnassignReportsAsync(
        PathologyParcelKeyDto key,
        PathologyParcelAssignReportsRequestDto request,
        CancellationToken cancellationToken = default);
}