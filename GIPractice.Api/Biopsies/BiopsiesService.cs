using GIPractice.Contracts.Biopsies;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;

namespace GIPractice.Api.Biopsies;

public sealed class BiopsiesService : IBiopsiesService
{
    private readonly IBiopsiesStore _store;

    public BiopsiesService(IBiopsiesStore store) => _store = store;

    public async Task<ResultDto<PagedResultDto<BiopsyBottleDto>>> SearchBottlesAsync(BiopsyBottleSearchRequestDto request, CancellationToken cancellationToken = default)
        => ResultDto<PagedResultDto<BiopsyBottleDto>>.Ok(await _store.SearchBottlesAsync(request, cancellationToken));

    public async Task<ResultDto<BiopsyBottleDto>> GetBottleAsync(BiopsyBottleId id, CancellationToken cancellationToken = default)
    {
        var item = await _store.GetBottleAsync(id, cancellationToken);
        return item is null
            ? ResultDto<BiopsyBottleDto>.Fail("not_found", "Biopsy bottle not found.")
            : ResultDto<BiopsyBottleDto>.Ok(item);
    }

    public Task<ResultDto<BiopsyBottleId>> CreateBottleAsync(BiopsyBottleUpsertRequestDto request, CancellationToken cancellationToken = default)
        => _store.CreateBottleAsync(request, cancellationToken);

    public Task<ResultDto<bool>> UpdateBottleAsync(BiopsyBottleUpsertRequestDto request, CancellationToken cancellationToken = default)
        => _store.UpdateBottleAsync(request, cancellationToken);

    public Task<ResultDto<bool>> DeleteBottleAsync(BiopsyBottleId id, CancellationToken cancellationToken = default)
        => _store.DeleteBottleAsync(id, cancellationToken);

    public async Task<ResultDto<PagedResultDto<BiopsyDispatchBundleDto>>> SearchDispatchBundlesAsync(BiopsyDispatchSearchRequestDto request, CancellationToken cancellationToken = default)
        => ResultDto<PagedResultDto<BiopsyDispatchBundleDto>>.Ok(await _store.SearchDispatchBundlesAsync(request, cancellationToken));

    public async Task<ResultDto<BiopsyDispatchDetailsDto>> GetDispatchDetailsAsync(BiopsyDispatchBundleId id, CancellationToken cancellationToken = default)
    {
        var item = await _store.GetDispatchDetailsAsync(id, cancellationToken);
        return item is null
            ? ResultDto<BiopsyDispatchDetailsDto>.Fail("not_found", "Dispatch bundle not found.")
            : ResultDto<BiopsyDispatchDetailsDto>.Ok(item);
    }

    public Task<ResultDto<BiopsyDispatchBundleId>> CreateDispatchBundleAsync(BiopsyDispatchCreateRequestDto request, CancellationToken cancellationToken = default)
        => _store.CreateDispatchBundleAsync(request, cancellationToken);

    public Task<ResultDto<bool>> CloseDispatchBundleAsync(BiopsyDispatchCloseRequestDto request, CancellationToken cancellationToken = default)
        => _store.CloseDispatchBundleAsync(request, cancellationToken);
}
