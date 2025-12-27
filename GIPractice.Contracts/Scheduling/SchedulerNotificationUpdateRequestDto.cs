using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public enum SchedulerNotificationCloseOutcome { KeepOpen, Done, Cancelled, ConvertedToAppointment }

public sealed record SchedulerNotificationUpdateRequestDto(
    SchedulerNotificationId Id,
    DateTime? SnoozedUntilUtc,
    SchedulerNotificationCloseOutcome Outcome,
    string? Notes,
    byte[]? RowVersion);