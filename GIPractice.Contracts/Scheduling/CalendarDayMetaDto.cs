namespace GIPractice.Contracts.Scheduling;

public sealed record CalendarDayMetaDto(
    DateOnly Day,
    bool IsHoliday,
    bool IsDayOff,
    string? Notes,
    byte[]? RowVersion);