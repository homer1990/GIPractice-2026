using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public sealed record AppointmentRescheduleRequestDto(
    AppointmentId AppointmentId,
    DateTime NewStartUtc,
    int NewDurationMinutes,
    string? Notes);
