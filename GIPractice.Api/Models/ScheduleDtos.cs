using GIPractice.Core.Enums;

namespace GIPractice.Api.Models;

public enum ScheduleItemType
{
    Appointment = 1,
    Visit = 2
}

public class ScheduleItemDto
{
    /// <summary>
    /// Appointment or Visit.
    /// </summary>
    public ScheduleItemType Type { get; set; }

    /// <summary>
    /// Underlying entity Id (Appointment.Id or Visit.Id).
    /// </summary>
    public int EntityId { get; set; }

    public int PatientId { get; set; }
    public string PatientFullName { get; set; } = string.Empty;

    /// <summary>
    /// For appointments: StartDateTimeUtc.
    /// For visits: DateOfVisitUtc.
    /// </summary>
    public DateTime StartUtc { get; set; }

    /// <summary>
    /// For appointments: EndDateTimeUtc; for visits usually null.
    /// </summary>
    public DateTime? EndUtc { get; set; }

    // Appointment-specific metadata (null for visits)
    public bool? Canceled { get; set; }
    public bool? TookPlace { get; set; }
    public bool? Urgent { get; set; }
    public AppointmentPurpose? AppointmentPurpose { get; set; }
    public EndoscopyType? PlannedEndoscopyType { get; set; }

    /// <summary>
    /// Free-text notes from Appointment/Visit.
    /// </summary>
    public string? Notes { get; set; }
}

public class ScheduleRangeDto
{
    public DateTime FromUtc { get; set; }
    public DateTime ToUtc { get; set; }

    public List<ScheduleItemDto> Items { get; set; } = [];
}
