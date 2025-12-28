using System.ComponentModel.DataAnnotations;

namespace GIPractice.Contracts.Settings;

public sealed record AppSettingUpsertRequestDto(
    [param: Required, MaxLength(200)] string Key,
    [param: Required, MaxLength(4000)] string Value,
    [param: EnumDataType(typeof(AppSettingScope))] AppSettingScope Scope,
    [param: MaxLength(200)] string? ScopeKey,
    [param: MaxLength(500)] string? Description,
    byte[]? RowVersion);
