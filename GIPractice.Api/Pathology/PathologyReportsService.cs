using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;

namespace GIPractice.Api.Pathology;

public sealed class PathologyReportsService(IPathologyReportsStore store) : IPathologyReportsService
{
    public Task<ResultDto<PagedResultDto<PathologyReportListItemDto>>> SearchAsync(PathologyReportSearchRequestDto request, CancellationToken cancellationToken = default)
        => store.SearchAsync(request, cancellationToken);

    public Task<ResultDto<PathologyReportDto>> GetAsync(PathologyReportId id, CancellationToken cancellationToken = default)
        => store.GetAsync(id, cancellationToken);

    public Task<ResultDto<PathologyReportId>> CreateAsync(PathologyReportUpsertRequestDto request, CancellationToken cancellationToken = default)
        => store.CreateAsync(request, cancellationToken);

    public Task<ResultDto<bool>> UpdateAsync(PathologyReportUpsertRequestDto request, CancellationToken cancellationToken = default)
        => store.UpdateAsync(request, cancellationToken);

    public Task<ResultDto<bool>> DeleteAsync(PathologyReportId id, CancellationToken cancellationToken = default)
        => store.DeleteAsync(id, cancellationToken);
}
