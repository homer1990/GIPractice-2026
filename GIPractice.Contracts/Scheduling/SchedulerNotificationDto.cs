using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public enum SchedulerNotificationKind
{
    // Scheduling / contact workflow
    OpenRescheduleContact = 1,     // contact patient to reschedule an existing appointment
    PatientReexamDue = 2,          // patient needs scheduling due to re-exam date or plan
    DoctorRequestedScheduling = 3, // explicit manual “schedule this” task

    // Results / follow-ups
    PathologyReadyNotInformed = 10,
    InfaiReportReadyNotInformed = 11,

    // Operations
    BiopsyDispatchPending = 20,    // e.g. bottles collected but dispatch bundle not created/sent
}

public enum SchedulerNotificationSeverity
{
    Info = 0,
    Normal = 1,
    Important = 2,
    Urgent = 3
}

public sealed record SchedulerNotificationDto(
    SchedulerNotificationId Id,
    SchedulerNotificationKind Kind,
    SchedulerNotificationSeverity Severity,

    PatientId PatientId,
    string PatientFullName,
    string? PatientPhoneNumber,

    DateTime CreatedUtc,
    DateTime DueUtc,
    DateTime? SnoozedUntilUtc,
    bool IsClosed,
    DateTime? ClosedUtc,

    string? Notes,

    // Optional links (filled depending on Kind)
    AppointmentId? AppointmentId = null,
    EncounterId? EncounterId = null,
    EndoscopyId? EndoscopyId = null,
    PathologyReportId? PathologyReportId = null,
    InfaiReportId? InfaiReportId = null,
    BiopsyDispatchBundleId? BiopsyDispatchBundleId = null,

    // Optional “what should we schedule”
    AppointmentTypeId? SuggestedAppointmentTypeId = null,
    EncounterTypeId? SuggestedEncounterTypeId = null,
    DateOnly? SuggestedDay = null
);