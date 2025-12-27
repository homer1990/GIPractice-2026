using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

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