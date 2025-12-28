using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public enum SchedulerNotificationCloseOutcome { KeepOpen, Done, Cancelled, ConvertedToAppointment }

public sealed record SchedulerNotificationUpdateRequestDto(
    [NonZeroId] SchedulerNotificationId Id,
    DateTime? SnoozedUntilUtc,
    [EnumDataType(typeof(SchedulerNotificationCloseOutcome))] SchedulerNotificationCloseOutcome Outcome,
    [MaxLength(2000)] string? Notes,
    byte[]? RowVersion);
