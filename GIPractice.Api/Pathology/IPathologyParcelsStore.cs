using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;

namespace GIPractice.Api.Pathology;

public interface IPathologyParcelsStore
{
    Task<ResultDto<PagedResultDto<PathologyParcelDto>>> SearchAsync(PathologyParcelSearchRequestDto request, CancellationToken ct = default);
    Task<ResultDto<PathologyParcelDto>> GetByKeyAsync(PathologyParcelKeyDto key, CancellationToken ct = default);
    Task<ResultDto<PathologyParcelId>> CreateAsync(PathologyParcelCreateRequestDto request, CancellationToken ct = default);
    Task<ResultDto<bool>> UpdateByKeyAsync(PathologyParcelKeyDto key, PathologyParcelUpdateRequestDto request, CancellationToken ct = default);
    Task<ResultDto<bool>> AssignReportsAsync(PathologyParcelKeyDto key, PathologyParcelAssignReportsRequestDto request, CancellationToken ct = default);
    Task<ResultDto<bool>> UnassignReportsAsync(PathologyParcelKeyDto key, PathologyParcelAssignReportsRequestDto request, CancellationToken ct = default);
}
