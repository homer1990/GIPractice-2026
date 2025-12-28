using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public sealed record OpenRescheduleCreateRequestDto(
    [NonZeroId] AppointmentTypeId AppointmentId,
    DateTime? NextContactUtc,
    [MaxLength(2000)] string? Notes);
