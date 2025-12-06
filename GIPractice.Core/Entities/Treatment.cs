using GIPractice.Core.Abstractions;
using GIPractice.Core.Enums;

namespace GIPractice.Core.Entities;

public class Treatment : BaseEntity
{
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public DateTime StartUtc { get; set; }
    public DateTime? EndUtc { get; set; }

    public Outcomes Outcome { get; set; } = Outcomes.None;

    public bool IsChronic { get; set; } = false;

    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public string? Notes { get; set; }

    public List<Diagnosis> Diagnoses { get; set; } = [];
    public List<Medicine> Medications { get; set; } = [];
}
