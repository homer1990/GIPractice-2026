using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public sealed record AppointmentRescheduleRequestDto(
    [NonZeroId] AppointmentId AppointmentId,
    [NotDefault] DateTime NewStartUtc,
    [Range(5, 24 * 60)] int NewDurationMinutes,
    [MaxLength(2000)] string? Notes);
