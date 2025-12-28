using GIPractice.Contracts.Common;

namespace GIPractice.Contracts.Settings;

public interface ISettingsService
{
    Task<ResultDto<PagedResultDto<AppSettingListItemDto>>> SearchAsync(
        AppSettingSearchRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<AppSettingDto>> GetAsync(
        string key,
        AppSettingScope scope,
        string? scopeKey,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> UpsertAsync(
        AppSettingUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> DeleteAsync(
        string key,
        AppSettingScope scope,
        string? scopeKey,
        CancellationToken cancellationToken = default);
}
