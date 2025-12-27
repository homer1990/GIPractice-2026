using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public sealed record AppointmentTypeDto(
    AppointmentTypeId Id,
    string Name,
    int MeanDurationMinutes,
    string? ColorHex);
