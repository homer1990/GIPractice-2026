using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;

namespace GIPractice.Api.Pathology;

public interface IPathologyStore
{
    Task<PagedResultDto<PathologyReportListItemDto>> SearchAsync(PathologyReportSearchRequestDto request, CancellationToken ct);
    Task<PathologyReportDto?> GetAsync(PathologyReportId id, CancellationToken ct);
    Task<ResultDto<PathologyReportId>> CreateAsync(PathologyReportUpsertRequestDto request, CancellationToken ct);
    Task<ResultDto<bool>> UpdateAsync(PathologyReportUpsertRequestDto request, CancellationToken ct);
    Task<ResultDto<bool>> DeleteAsync(PathologyReportId id, CancellationToken ct);
}
