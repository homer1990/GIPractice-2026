namespace GIPractice.Contracts.Common;

public sealed record PagedRequestDto(
    int Page = 1,
    int PageSize = 50,
    SortDto? Sort = null);
