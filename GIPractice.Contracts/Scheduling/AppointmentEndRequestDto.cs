using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public sealed record AppointmentEndRequestDto(
    EncounterId EncounterId,
    DateTime ActualEndUtc);
