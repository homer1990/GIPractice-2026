using GIPractice.Core.Enums;

namespace GIPractice.Api.Models;

public class EndoscopyCreateDto
{
    public int VisitId { get; set; }

    public EndoscopyType Type { get; set; }
    public DateTime? PerformedAtUtc { get; set; }
    public bool IsUrgent { get; set; }

    public string? Notes { get; set; }
}

public class EndoscopyDto
{
    public int Id { get; set; }

    public int VisitId { get; set; }
    public int PatientId { get; set; }

    public EndoscopyType Type { get; set; }
    public DateTime PerformedAtUtc { get; set; }
    public bool IsUrgent { get; set; }

    public string? Notes { get; set; }

    public List<ObservationDto> Observations { get; set; } = [];
    public List<BiopsyBottleDto> BiopsyBottles { get; set; } = [];
    public ReportDto? Report { get; set; }
    public List<MediaFileDto> Media { get; set; } = [];
}

public class EndoscopyListItemDto
{
    public int Id { get; set; }

    public int PatientId { get; set; }
    public string PatientFullName { get; set; } = string.Empty;

    public int VisitId { get; set; }

    public EndoscopyType Type { get; set; }
    public DateTime PerformedAtUtc { get; set; }
    public bool IsUrgent { get; set; }

    public string? Notes { get; set; }

    public int BiopsyBottlesCount { get; set; }
    public bool HasReport { get; set; }
}
