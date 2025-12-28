using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Biopsies;

public interface IBiopsiesService
{
    // Bottles
    Task<ResultDto<PagedResultDto<BiopsyBottleDto>>> SearchBottlesAsync(
        BiopsyBottleSearchRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<BiopsyBottleDto>> GetBottleAsync(
        BiopsyBottleId id,
        CancellationToken cancellationToken = default);

    Task<ResultDto<BiopsyBottleId>> CreateBottleAsync(
        BiopsyBottleUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> UpdateBottleAsync(
        BiopsyBottleUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> DeleteBottleAsync(
        BiopsyBottleId id,
        CancellationToken cancellationToken = default);

    // Dispatch bundles
    Task<ResultDto<PagedResultDto<BiopsyDispatchBundleDto>>> SearchDispatchBundlesAsync(
        BiopsyDispatchSearchRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<BiopsyDispatchDetailsDto>> GetDispatchDetailsAsync(
        BiopsyDispatchBundleId id,
        CancellationToken cancellationToken = default);

    Task<ResultDto<BiopsyDispatchBundleId>> CreateDispatchBundleAsync(
        BiopsyDispatchCreateRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> CloseDispatchBundleAsync(
        BiopsyDispatchCloseRequestDto request,
        CancellationToken cancellationToken = default);
}
