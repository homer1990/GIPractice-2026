namespace GIPractice.Contracts.Localization;

public sealed record TranslationListItemDto(
    string Key,
    string Culture,
    string? Module);
