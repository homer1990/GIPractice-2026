using System.ComponentModel.DataAnnotations;

namespace GIPractice.Contracts.Localization;

public sealed record TranslationUpsertRequestDto(
    [property: Required, MaxLength(200)] string Key,
    [property: Required, MaxLength(16)] string Culture,
    [property: Required, MaxLength(4000)] string Value,
    [property: MaxLength(64)] string? Module,
    [property: MaxLength(2000)] string? Notes,
    byte[]? RowVersion);
