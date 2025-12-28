using GIPractice.Contracts.Common;
using GIPractice.Contracts.Localization;

namespace GIPractice.Api.Localization;

public sealed class LocalizationService : ILocalizationService
{
    private readonly ILocalizationStore _store;

    public LocalizationService(ILocalizationStore store) => _store = store;

    public async Task<ResultDto<PagedResultDto<TranslationListItemDto>>> SearchAsync(TranslationSearchRequestDto request, CancellationToken cancellationToken = default)
        => ResultDto<PagedResultDto<TranslationListItemDto>>.Ok(await _store.SearchAsync(request, cancellationToken));

    public async Task<ResultDto<TranslationDto>> GetAsync(string key, string culture, CancellationToken cancellationToken = default)
    {
        var item = await _store.GetAsync(key, culture, cancellationToken);
        return item is null
            ? ResultDto<TranslationDto>.Fail("not_found", "Translation not found.")
            : ResultDto<TranslationDto>.Ok(item);
    }

    public Task<ResultDto<bool>> UpsertAsync(TranslationUpsertRequestDto request, CancellationToken cancellationToken = default)
        => _store.UpsertAsync(request, cancellationToken);

    public Task<ResultDto<bool>> DeleteAsync(string key, string culture, CancellationToken cancellationToken = default)
        => _store.DeleteAsync(key, culture, cancellationToken);
}
