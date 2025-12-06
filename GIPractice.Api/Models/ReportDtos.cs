namespace GIPractice.Api.Models;

public class ReportCreateDto
{
    public int EndoscopyId { get; set; }
    public bool IsUrgent { get; set; }

    public int? FileId { get; set; }
    public string? Summary { get; set; }
}

public class ReportDto
{
    public int Id { get; set; }
    public bool IsUrgent { get; set; }

    public int? FileId { get; set; }
    public string? Summary { get; set; }

    public List<BiopsyBottleDto> BiopsyBottles { get; set; } = [];
}
