using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public sealed record OpenRescheduleUpdateRequestDto(
    [NonZeroId] AppointmentId AppointmentId,
    DateTime? NextContactUtc,
    [MaxLength(2000)] string? Notes,
    [EnumDataType(typeof(OpenRescheduleCloseOutcome))] OpenRescheduleCloseOutcome Outcome);
