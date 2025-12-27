namespace GIPractice.Contracts.Common;

public sealed record SortDto(
    string Field,
    bool Desc = false);