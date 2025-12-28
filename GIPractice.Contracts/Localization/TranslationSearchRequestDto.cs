using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common;

namespace GIPractice.Contracts.Localization;

public sealed record TranslationSearchRequestDto(
    [param: MaxLength(200)] string? KeyContains = null,
    [param: MaxLength(16)] string? Culture = null,
    [param: MaxLength(64)] string? Module = null,
    PagedRequestDto? Paging = null);
