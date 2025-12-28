using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public sealed record AppointmentEndRequestDto(
    [NonZeroId] EncounterId EncounterId,
    [NotDefault] DateTime ActualEndUtc);
