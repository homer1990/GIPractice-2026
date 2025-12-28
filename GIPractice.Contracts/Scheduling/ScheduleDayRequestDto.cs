namespace GIPractice.Contracts.Scheduling;

public sealed record ScheduleDayRequestDto(
    [NotDefault] DateOnly Date);
