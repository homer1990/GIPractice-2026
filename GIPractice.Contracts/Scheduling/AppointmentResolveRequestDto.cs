using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public sealed record AppointmentResolveRequestDto(
    AppointmentId AppointmentId,
    DateTime ActualStartUtc);
