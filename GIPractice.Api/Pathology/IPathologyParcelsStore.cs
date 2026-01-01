using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;

namespace GIPractice.Api.Pathology;

public interface IPathologyParcelsStore
{
    Task<ResultDto<PagedResultDto<PathologyParcelDto>>> SearchAsync(PathologyParcelSearchRequestDto request, CancellationToken ct = default);
    Task<ResultDto<PathologyParcelDto>> GetByKeyAsync(PathologyParcelKeyDto key, CancellationToken ct = default);
    Task<ResultDto<PathologyParcelDto>> GetAsync(PathologyParcelId id, CancellationToken ct = default);
    Task<ResultDto<PathologyParcelId>> CreateAsync(PathologyParcelCreateRequestDto request, CancellationToken ct = default);
    Task<ResultDto<bool>> UpdateByKeyAsync(PathologyParcelKeyDto key, PathologyParcelUpdateRequestDto request, CancellationToken ct = default);
    Task<ResultDto<bool>> AssignEndoscopiesAsync(PathologyParcelKeyDto key, PathologyParcelAssignEndoscopiesRequestDto request, CancellationToken ct = default);
    Task<ResultDto<bool>> UnassignEndoscopiesAsync(PathologyParcelKeyDto key, PathologyParcelAssignEndoscopiesRequestDto request, CancellationToken ct = default);
}
