using GIPractice.Core.Abstractions;
using GIPractice.Core.Enums;

namespace GIPractice.Core.Entities;

public class InfaiTest : BaseEntity
{
    public int EncounterId { get; set; }
    public Encounter Encounter { get; set; } = null!;

    public string? Notes { get; set; }
    public QualitativeTestResults Result { get; set; } = QualitativeTestResults.Indeterminate;

    public int? FileId { get; set; }
    public MediaFile? File { get; set; }
}
