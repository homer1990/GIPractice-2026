using GIPractice.Core.Abstractions;
using GIPractice.Core.Entities;

public class Visit : BaseEntity
{
    public DateTime DateOfVisitUtc { get; set; }
    public string? Notes { get; set; }

    public int? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public List<Endoscopy> Endoscopies { get; set; } = [];
    public List<Diagnosis> Diagnoses { get; set; } = [];
}
