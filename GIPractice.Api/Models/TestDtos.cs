using GIPractice.Core.Enums;

namespace GIPractice.Api.Models;

public class TestCreateDto
{
    public int PatientId { get; set; }

    public TestType TestType { get; set; } = TestType.None;

    public DateTime PerformedAtUtc { get; set; }

    public QualitativeTestResults QualitativeResult { get; set; }
        = QualitativeTestResults.None;

    public decimal? QuantitativeResult { get; set; }

    public int? LabId { get; set; }
    public int? DoctorId { get; set; }
    public int? FileId { get; set; }

    public string? Notes { get; set; }

    /// <summary>Linked findings (e.g. positive markers).</summary>
    public List<int> FindingIds { get; set; } = [];
}

public class TestDto : TestCreateDto
{
    public int Id { get; set; }

    public string PatientFullName { get; set; } = string.Empty;
    public string? LabName { get; set; }
    public string? DoctorFullName { get; set; }
}
