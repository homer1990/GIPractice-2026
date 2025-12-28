using System.ComponentModel.DataAnnotations;

namespace GIPractice.Contracts.Settings;

public sealed record AppSettingUpsertRequestDto(
    [property: Required, MaxLength(200)] string Key,
    [property: Required, MaxLength(4000)] string Value,
    [property: EnumDataType(typeof(AppSettingScope))] AppSettingScope Scope,
    [property: MaxLength(200)] string? ScopeKey,
    [property: MaxLength(500)] string? Description,
    byte[]? RowVersion);
