using GIPractice.Core.Abstractions;
using GIPractice.Core.Enums;

namespace GIPractice.Core.Entities;

public class Diagnosis : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    public DiagnosisStandard Standard { get; set; } = DiagnosisStandard.None;

    // Navigation
    public List<Patient> Patients { get; set; } = [];
    public List<Visit> Visits { get; set; } = [];
    public List<Endoscopy> Endoscopies { get; set; } = [];
    public List<Treatment> Treatments { get; set; } = [];
}
