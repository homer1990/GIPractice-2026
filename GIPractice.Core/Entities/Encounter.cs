using GIPractice.Core.Abstractions;
using GIPractice.Core.Enums;

namespace GIPractice.Core.Entities;

/// <summary>
/// The clinical event root. Every visit, endoscopy, clinical exam and INFAI
/// procedure belongs to exactly one encounter. Patient, timing, urgency and
/// appointment linkage live here so detail tables cannot disagree about them.
/// </summary>
public class Encounter : BaseEntity
{
    public EncounterKind Kind { get; set; }
    public EncounterStatus Status { get; set; } = EncounterStatus.InProgress;

    public DateTime StartedAtUtc { get; set; }
    public DateTime? EndedAtUtc { get; set; }
    public bool IsUrgent { get; set; }
    public string? Notes { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int? AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }

    public Visit? Visit { get; set; }
    public Endoscopy? Endoscopy { get; set; }
    public Exam? Exam { get; set; }
    public InfaiTest? Infai { get; set; }
}
