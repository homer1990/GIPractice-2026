namespace GIPractice.Api.Models;

public class BiopsyBottleCreateDto
{
    public int EndoscopyId { get; set; }

    public string Label { get; set; } = string.Empty;
    public int Number { get; set; }

    public List<int> OrganAreaIds { get; set; } = [];
}

public class BiopsyBottleDto
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public int Number { get; set; }

    public List<OrganAreaDto> OrganAreas { get; set; } = [];
}

public class OrganAreaDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
