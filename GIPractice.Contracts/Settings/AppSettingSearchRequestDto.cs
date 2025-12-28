using GIPractice.Contracts.Common;

namespace GIPractice.Contracts.Settings;

public sealed record AppSettingSearchRequestDto(
    string? KeyContains = null,
    AppSettingScope? Scope = null,
    string? ScopeKey = null,
    PagedRequestDto? Paging = null);
