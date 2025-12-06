using GIPractice.Core.Enums;

namespace GIPractice.Api.Models;

public class DiagnosisCreateDto
{
    public string Name { get; set; } = string.Empty;

    /// <summary>ICD-10, SNOMED, local etc.</summary>
    public string Code { get; set; } = string.Empty;

    public DiagnosisStandard Standard { get; set; } = DiagnosisStandard.None;
}

public class DiagnosisDto : DiagnosisCreateDto
{
    public int Id { get; set; }
}
