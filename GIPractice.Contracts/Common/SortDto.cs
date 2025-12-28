namespace GIPractice.Contracts.Common;

public sealed record SortDto(
    [Required, MaxLength(64)] string Field,
    bool Desc = false);
