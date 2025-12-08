// PagedRequestDto.cs
namespace GIPractice.Api.Models;

public abstract class PagedRequestDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortField { get; set; }
    public bool SortDescending { get; set; }
}
