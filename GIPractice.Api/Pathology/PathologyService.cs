using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;

namespace GIPractice.Api.Pathology;

public sealed class PathologyService : IPathologyService
{
    private readonly IPathologyStore _store;

    public PathologyService(IPathologyStore store) => _store = store;

    public async Task<ResultDto<PagedResultDto<PathologyReportListItemDto>>> SearchAsync(PathologyReportSearchRequestDto request, CancellationToken cancellationToken = default)
        => ResultDto<PagedResultDto<PathologyReportListItemDto>>.Ok(await _store.SearchAsync(request, cancellationToken));

    public async Task<ResultDto<PathologyReportDto>> GetAsync(PathologyReportId id, CancellationToken cancellationToken = default)
    {
        var item = await _store.GetAsync(id, cancellationToken);
        return item is null
            ? ResultDto<PathologyReportDto>.Fail("not_found", "Pathology report not found.")
            : ResultDto<PathologyReportDto>.Ok(item);
    }

    public Task<ResultDto<PathologyReportId>> CreateAsync(PathologyReportUpsertRequestDto request, CancellationToken cancellationToken = default)
        => _store.CreateAsync(request, cancellationToken);

    public Task<ResultDto<bool>> UpdateAsync(PathologyReportUpsertRequestDto request, CancellationToken cancellationToken = default)
        => _store.UpdateAsync(request, cancellationToken);

    public Task<ResultDto<bool>> DeleteAsync(PathologyReportId id, CancellationToken cancellationToken = default)
        => _store.DeleteAsync(id, cancellationToken);
}
