using GIPractice.Contracts.Common;
using GIPractice.Contracts.Localization;

namespace GIPractice.Api.Localization;

public interface ILocalizationStore
{
    Task<PagedResultDto<TranslationListItemDto>> SearchAsync(TranslationSearchRequestDto request, CancellationToken ct);
    Task<TranslationDto?> GetAsync(string key, string culture, CancellationToken ct);
    Task<ResultDto<bool>> UpsertAsync(TranslationUpsertRequestDto request, CancellationToken ct);
    Task<ResultDto<bool>> DeleteAsync(string key, string culture, CancellationToken ct);
}
