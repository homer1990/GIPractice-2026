namespace GIPractice.Contracts.Scheduling;

public sealed record ScheduleDayDto(
    CalendarDayMetaDto DayMeta,
    IReadOnlyList<AppointmentTypeDto> AppointmentTypes,
    IReadOnlyList<AppointmentListItemDto> Appointments,
    IReadOnlyList<OpenRescheduleItemDto> OpenRescheduleQueue,
    IReadOnlyList<SchedulerNotificationDto> Notifications);
