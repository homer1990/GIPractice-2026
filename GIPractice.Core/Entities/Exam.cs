using GIPractice.Core.Abstractions;

namespace GIPractice.Core.Entities;

/// <summary>Clinical examination details for an Encounter of kind Exam.</summary>
public class Exam : BaseEntity
{
    public int EncounterId { get; set; }
    public Encounter Encounter { get; set; } = null!;

    public bool SeriousFindings { get; set; }
    public string? Notes { get; set; }
}
