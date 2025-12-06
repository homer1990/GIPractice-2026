// GIPractice.Core/Entities/BiopsyBottle.cs

using GIPractice.Core.Abstractions;

namespace GIPractice.Core.Entities;

public class BiopsyBottle : BaseEntity
{
    public int EndoscopyId { get; set; }
    public Endoscopy Endoscopy { get; set; } = null!;

    // Direct chain-of-custody to patient
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    /// <summary>Moment of collection (usually same as endoscopy time).</summary>
    public DateTime CollectedAtUtc { get; set; }

    public string Label { get; set; } = string.Empty;
    public int Number { get; set; }

    public List<OrganArea> OrganAreas { get; set; } = new();
}
