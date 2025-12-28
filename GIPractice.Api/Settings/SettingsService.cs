using GIPractice.Contracts.Common;
using GIPractice.Contracts.Settings;

namespace GIPractice.Api.Settings;

public sealed class SettingsService : ISettingsService
{
    private readonly ISettingsStore _store;

    public SettingsService(ISettingsStore store) => _store = store;

    public async Task<ResultDto<PagedResultDto<AppSettingListItemDto>>> SearchAsync(AppSettingSearchRequestDto request, CancellationToken cancellationToken = default)
        => ResultDto<PagedResultDto<AppSettingListItemDto>>.Ok(await _store.SearchAsync(request, cancellationToken));

    public async Task<ResultDto<AppSettingDto>> GetAsync(string key, AppSettingScope scope, string? scopeKey, CancellationToken cancellationToken = default)
    {
        var item = await _store.GetAsync(key, scope, scopeKey, cancellationToken);
        return item is null
            ? ResultDto<AppSettingDto>.Fail("not_found", "Setting not found.")
            : ResultDto<AppSettingDto>.Ok(item);
    }

    public Task<ResultDto<bool>> UpsertAsync(AppSettingUpsertRequestDto request, CancellationToken cancellationToken = default)
        => _store.UpsertAsync(request, cancellationToken);

    public Task<ResultDto<bool>> DeleteAsync(string key, AppSettingScope scope, string? scopeKey, CancellationToken cancellationToken = default)
        => _store.DeleteAsync(key, scope, scopeKey, cancellationToken);
}
