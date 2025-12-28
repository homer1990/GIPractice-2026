namespace GIPractice.Contracts.Settings;

public sealed record AppSettingListItemDto(
    string Key,
    AppSettingScope Scope,
    string? ScopeKey,
    string? Description);
