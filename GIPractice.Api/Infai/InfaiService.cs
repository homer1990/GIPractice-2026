using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Infai;

namespace GIPractice.Api.Infai;

public sealed class InfaiService : IInfaiService
{
    private readonly IInfaiStore _store;

    public InfaiService(IInfaiStore store) => _store = store;

    public async Task<ResultDto<PagedResultDto<InfaiReportListItemDto>>> SearchAsync(InfaiReportSearchRequestDto request, CancellationToken cancellationToken = default)
        => ResultDto<PagedResultDto<InfaiReportListItemDto>>.Ok(await _store.SearchAsync(request, cancellationToken));

    public async Task<ResultDto<InfaiReportDto>> GetAsync(InfaiReportId id, CancellationToken cancellationToken = default)
    {
        var item = await _store.GetAsync(id, cancellationToken);
        return item is null
            ? ResultDto<InfaiReportDto>.Fail("not_found", "INFAI report not found.")
            : ResultDto<InfaiReportDto>.Ok(item);
    }

    public Task<ResultDto<InfaiReportId>> CreateAsync(InfaiReportUpsertRequestDto request, CancellationToken cancellationToken = default)
        => _store.CreateAsync(request, cancellationToken);

    public Task<ResultDto<bool>> UpdateAsync(InfaiReportUpsertRequestDto request, CancellationToken cancellationToken = default)
        => _store.UpdateAsync(request, cancellationToken);

    public Task<ResultDto<bool>> DeleteAsync(InfaiReportId id, CancellationToken cancellationToken = default)
        => _store.DeleteAsync(id, cancellationToken);
}
