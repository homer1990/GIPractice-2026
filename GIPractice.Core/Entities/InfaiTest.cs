using GIPractice.Core.Abstractions;
using GIPractice.Core.Enums;

namespace GIPractice.Core.Entities;

public class InfaiTest : BaseEntity
{
    public string? Notes { get; set; }

    public DateTime PerformedAtUtc { get; set; }

    public QualitativeTestResults Result { get; set; } = QualitativeTestResults.Indeterminate;

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int? FileId { get; set; }
    public MediaFile? File { get; set; }
}
