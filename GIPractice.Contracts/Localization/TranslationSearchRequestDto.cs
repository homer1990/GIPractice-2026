using GIPractice.Contracts.Common;

namespace GIPractice.Contracts.Localization;

public sealed record TranslationSearchRequestDto(
    string? KeyContains = null,
    string? Culture = null,
    string? Module = null,
    PagedRequestDto? Paging = null);
