using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public interface IPathologyService
{
    Task<ResultDto<PagedResultDto<PathologyReportListItemDto>>> SearchAsync(
        PathologyReportSearchRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<PathologyReportDto>> GetAsync(
        PathologyReportId id,
        CancellationToken cancellationToken = default);

    Task<ResultDto<PathologyReportId>> CreateAsync(
        PathologyReportUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> UpdateAsync(
        PathologyReportUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> DeleteAsync(
        PathologyReportId id,
        CancellationToken cancellationToken = default);
}
