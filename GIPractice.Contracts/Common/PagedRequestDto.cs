namespace GIPractice.Contracts.Common;

public sealed record PagedRequestDto(
    [Range(1, int.MaxValue)] int Page = 1,
    [Range(1, 500)] int PageSize = 50,
    SortDto? Sort = null);
