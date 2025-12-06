using GIPractice.Core.Abstractions;

namespace GIPractice.Core.Entities;

public class Report : BaseEntity
{
    public bool IsUrgent { get; set; }

    public int EndoscopyId { get; set; }
    public Endoscopy Endoscopy { get; set; } = null!;

    public int? FileId { get; set; }
    public MediaFile? File { get; set; }

    public List<BiopsyBottle> BiopsyBottles { get; set; } = [];

    public string? Summary { get; set; }
}
