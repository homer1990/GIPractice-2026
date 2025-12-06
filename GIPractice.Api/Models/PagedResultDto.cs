namespace GIPractice.Api.Models;

public class PagedResultDto<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public IReadOnlyList<T> Items { get; set; } = [];
}
