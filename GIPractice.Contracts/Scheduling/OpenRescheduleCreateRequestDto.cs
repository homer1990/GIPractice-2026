using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public sealed record OpenRescheduleCreateRequestDto(
    AppointmentTypeId AppointmentId,
    DateTime? NextContactUtc,
    string? Notes);