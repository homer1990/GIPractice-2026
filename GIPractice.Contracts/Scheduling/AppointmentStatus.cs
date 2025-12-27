namespace GIPractice.Contracts.Scheduling;

public enum AppointmentStatus
{
    Scheduled = 0,
    RescheduleRequested = 1,
    Rescheduled = 2,
    Cancelled = 3,
    NoShow = 4,
    Resolved = 5
}
