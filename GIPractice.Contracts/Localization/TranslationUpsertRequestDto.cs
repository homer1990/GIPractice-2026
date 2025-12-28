namespace GIPractice.Contracts.Localization;

public sealed record TranslationUpsertRequestDto(
    string Key,
    string Culture,
    string Value,
    string? Module,
    string? Notes,
    byte[]? RowVersion);
