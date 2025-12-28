using System.ComponentModel.DataAnnotations;

namespace GIPractice.Contracts.Localization;

public sealed record TranslationUpsertRequestDto(
    [param: Required, MaxLength(200)] string Key,
    [param: Required, MaxLength(16)] string Culture,
    [param: Required, MaxLength(4000)] string Value,
    [param: MaxLength(64)] string? Module,
    [param: MaxLength(2000)] string? Notes,
    byte[]? RowVersion);
