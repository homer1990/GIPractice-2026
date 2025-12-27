namespace GIPractice.Contracts.Scheduling;

public sealed record CalendarDayMetaUpsertDto(
    DateOnly Day,
    bool IsHoliday,
    bool IsDayOff,
    string? Notes);
