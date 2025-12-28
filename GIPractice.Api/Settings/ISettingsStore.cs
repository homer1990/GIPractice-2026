using GIPractice.Contracts.Common;
using GIPractice.Contracts.Settings;

namespace GIPractice.Api.Settings;

public interface ISettingsStore
{
    Task<PagedResultDto<AppSettingListItemDto>> SearchAsync(AppSettingSearchRequestDto request, CancellationToken ct);
    Task<AppSettingDto?> GetAsync(string key, AppSettingScope scope, string? scopeKey, CancellationToken ct);
    Task<ResultDto<bool>> UpsertAsync(AppSettingUpsertRequestDto request, CancellationToken ct);
    Task<ResultDto<bool>> DeleteAsync(string key, AppSettingScope scope, string? scopeKey, CancellationToken ct);
}
