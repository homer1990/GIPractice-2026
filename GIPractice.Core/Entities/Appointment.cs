using GIPractice.Core.Abstractions;
using GIPractice.Core.Enums;

namespace GIPractice.Core.Entities;

public class Appointment : BaseEntity
{
    public AppointmentPurpose Purpose { get; set; } = AppointmentPurpose.Consultation;
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    public DateTime StartDateTimeUtc { get; set; }
    public DateTime? EndDateTimeUtc { get; set; }
    public bool Urgent { get; set; }

    public EndoscopyType? PlannedEndoscopyType { get; set; }

    public int? PreparationProtocolId { get; set; }
    public PreparationProtocol? PreparationProtocol { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public Encounter? Encounter { get; set; }

    public string? Notes { get; set; }
}
