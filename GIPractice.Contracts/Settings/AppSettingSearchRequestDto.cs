using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common;

namespace GIPractice.Contracts.Settings;

public sealed record AppSettingSearchRequestDto(
    [param: MaxLength(200)] string? KeyContains = null,
    AppSettingScope? Scope = null,
    [param: MaxLength(200)] string? ScopeKey = null,
    PagedRequestDto? Paging = null);
