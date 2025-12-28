using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common;

namespace GIPractice.Contracts.Localization;

public sealed record TranslationSearchRequestDto(
    [property: MaxLength(200)] string? KeyContains = null,
    [property: MaxLength(16)] string? Culture = null,
    [property: MaxLength(64)] string? Module = null,
    PagedRequestDto? Paging = null);
