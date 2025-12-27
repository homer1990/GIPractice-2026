using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public sealed record OpenRescheduleUpdateRequestDto(
    AppointmentId AppointmentId,
    DateTime? NextContactUtc,
    string? Notes,
    OpenRescheduleCloseOutcome Outcome);
