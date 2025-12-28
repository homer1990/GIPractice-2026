using GIPractice.Contracts.Common;

namespace GIPractice.Contracts.Localization;

public interface ILocalizationService
{
    Task<ResultDto<PagedResultDto<TranslationListItemDto>>> SearchAsync(
        TranslationSearchRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<TranslationDto>> GetAsync(
        string key,
        string culture,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> UpsertAsync(
        TranslationUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> DeleteAsync(
        string key,
        string culture,
        CancellationToken cancellationToken = default);
}
