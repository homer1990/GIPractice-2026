using GIPractice.Core.Abstractions;
using GIPractice.Core.Enums;

namespace GIPractice.Core.Entities;

public class Observation : BaseEntity
{
    public int EndoscopyId { get; set; }
    public Endoscopy Endoscopy { get; set; } = null!;

    public int FindingId { get; set; }
    public Finding Finding { get; set; } = null!;

    public int OrganAreaId { get; set; }
    public OrganArea OrganArea { get; set; } = null!;

    public Severity Severity { get; set; }
    public Density Density { get; set; }

    public string Description { get; set; } = string.Empty;

    public int? EndoscopyMediaId { get; set; }
    public EndoscopyMedia? EndoscopyMedia { get; set; }
}
