namespace GIPractice.Contracts.Settings;

public sealed record AppSettingDto(
    string Key,
    string Value,
    AppSettingScope Scope,
    string? ScopeKey,     // e.g. userId, clinicId (string to avoid coupling to auth/user contracts)
    string? Description,
    byte[]? RowVersion);
