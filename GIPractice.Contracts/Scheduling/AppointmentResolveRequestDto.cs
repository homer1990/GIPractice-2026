using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public sealed record AppointmentResolveRequestDto(
    [NonZeroId] AppointmentId AppointmentId,
    [NotDefault] DateTime ActualStartUtc);
