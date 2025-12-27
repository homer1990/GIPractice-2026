namespace GIPractice.Contracts.Settings;

public sealed record AppSettingDto(
    string Key,
    string Value,
    string? Scope); // "Client" / "Server" / "User"
