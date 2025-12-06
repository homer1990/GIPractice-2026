namespace GIPractice.Api.Models;

/// <summary>
/// Used to attach/detach an existing diagnosis to/from a patient.
/// </summary>
public class PatientDiagnosisAssignDto
{
    public int DiagnosisId { get; set; }
}
