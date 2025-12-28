using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common;

namespace GIPractice.Contracts.Settings;

public sealed record AppSettingSearchRequestDto(
    [property: MaxLength(200)] string? KeyContains = null,
    AppSettingScope? Scope = null,
    [property: MaxLength(200)] string? ScopeKey = null,
    PagedRequestDto? Paging = null);
