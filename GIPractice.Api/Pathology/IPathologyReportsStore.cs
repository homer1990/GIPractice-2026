using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;

namespace GIPractice.Api.Pathology;

public interface IPathologyReportsStore
{
    Task<ResultDto<PagedResultDto<PathologyReportListItemDto>>> SearchAsync(PathologyReportSearchRequestDto request, CancellationToken ct = default);
    Task<ResultDto<PathologyReportDto>> GetAsync(PathologyReportId id, CancellationToken ct = default);
    Task<ResultDto<PathologyReportId>> CreateAsync(PathologyReportUpsertRequestDto request, CancellationToken ct = default);
    Task<ResultDto<bool>> UpdateAsync(PathologyReportUpsertRequestDto request, CancellationToken ct = default);
    Task<ResultDto<bool>> DeleteAsync(PathologyReportId id, CancellationToken ct = default);
}
