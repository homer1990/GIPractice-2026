namespace GIPractice.Contracts.Settings;

public sealed record AppSettingUpsertRequestDto(
    string Key,
    string Value,
    AppSettingScope Scope,
    string? ScopeKey,
    string? Description,
    byte[]? RowVersion);
