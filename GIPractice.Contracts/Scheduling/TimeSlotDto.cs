namespace GIPractice.Contracts.Scheduling;

public sealed record TimeSlotDto(
    DateTime StartUtc,
    DateTime EndUtc,
    bool IsAvailable,
    string? BlockReason);