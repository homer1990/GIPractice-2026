using GIPractice.Contracts.Biopsies;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;

namespace GIPractice.Api.Biopsies;

public interface IBiopsiesStore
{
    // Bottles
    Task<PagedResultDto<BiopsyBottleDto>> SearchBottlesAsync(BiopsyBottleSearchRequestDto request, CancellationToken ct);
    Task<BiopsyBottleDto?> GetBottleAsync(BiopsyBottleId id, CancellationToken ct);
    Task<ResultDto<BiopsyBottleId>> CreateBottleAsync(BiopsyBottleUpsertRequestDto request, CancellationToken ct);
    Task<ResultDto<bool>> UpdateBottleAsync(BiopsyBottleUpsertRequestDto request, CancellationToken ct);
    Task<ResultDto<bool>> DeleteBottleAsync(BiopsyBottleId id, CancellationToken ct);

    // Dispatch bundles
    Task<PagedResultDto<BiopsyDispatchBundleDto>> SearchDispatchBundlesAsync(BiopsyDispatchSearchRequestDto request, CancellationToken ct);
    Task<BiopsyDispatchDetailsDto?> GetDispatchDetailsAsync(BiopsyDispatchBundleId id, CancellationToken ct);
    Task<ResultDto<BiopsyDispatchBundleId>> CreateDispatchBundleAsync(BiopsyDispatchCreateRequestDto request, CancellationToken ct);
    Task<ResultDto<bool>> CloseDispatchBundleAsync(BiopsyDispatchCloseRequestDto request, CancellationToken ct);
}
