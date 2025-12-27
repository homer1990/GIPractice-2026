namespace GIPractice.Contracts.Scheduling;

public sealed record GetAvailableStartTimesResponseDto(
    IReadOnlyList<TimeSlotDto> Slots);
