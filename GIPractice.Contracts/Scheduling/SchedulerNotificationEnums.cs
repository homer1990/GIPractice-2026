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
