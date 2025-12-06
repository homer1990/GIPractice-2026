namespace GIPractice.Api.Models;

public class ActiveSubstanceDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}

public class ActiveSubstanceCreateUpdateDto
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}
