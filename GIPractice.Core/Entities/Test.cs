using GIPractice.Core.Abstractions;
using GIPractice.Core.Enums;

namespace GIPractice.Core.Entities;

public class Test : BaseEntity
{
    public string? Notes { get; set; }

    public DateTime PerformedAtUtc { get; set; }

    public TestType TestType { get; set; } = TestType.None;

    public QualitativeTestResults QualitativeResult { get; set; } = QualitativeTestResults.None;

    public decimal? QuantitativeResult { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int? LabId { get; set; }
    public Lab? Lab { get; set; }

    public int? DoctorId { get; set; }
    public Doctor? Doctor { get; set; }

    public int? FileId { get; set; }
    public MediaFile? File { get; set; }

    public List<Finding> Findings { get; set; } = [];
}
