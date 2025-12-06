using GIPractice.Core.Enums;

namespace GIPractice.Api.Models;

public class InfaiTestCreateDto
{
    public int PatientId { get; set; }

    public DateTime PerformedAtUtc { get; set; }

    public QualitativeTestResults Result { get; set; }
        = QualitativeTestResults.Indeterminate;

    public int? FileId { get; set; }

    public string? Notes { get; set; }
}

public class InfaiTestDto : InfaiTestCreateDto
{
    public int Id { get; set; }

    public string PatientFullName { get; set; } = string.Empty;
}
