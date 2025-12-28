namespace GIPractice.Contracts.Scheduling;

public sealed record CalendarDayMetaUpsertDto(
    [NotDefault] DateOnly Day,
    bool IsHoliday,
    bool IsDayOff,
    [MaxLength(2000)] string? Notes);
