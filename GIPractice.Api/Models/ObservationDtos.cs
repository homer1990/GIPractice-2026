using GIPractice.Core.Enums;

namespace GIPractice.Api.Models;

public class ObservationCreateDto
{
    public int EndoscopyId { get; set; }
    public int FindingId { get; set; }
    public int OrganAreaId { get; set; }

    public Severity Severity { get; set; }
    public Density Density { get; set; }

    public string Description { get; set; } = string.Empty;

    public int? MediaId { get; set; }
}

public class ObservationDto
{
    public int Id { get; set; }

    public int FindingId { get; set; }
    public string FindingName { get; set; } = string.Empty;

    public int OrganAreaId { get; set; }
    public string OrganAreaCode { get; set; } = string.Empty;

    public Severity Severity { get; set; }
    public Density Density { get; set; }

    public string Description { get; set; } = string.Empty;

    public int? MediaId { get; set; }
}
