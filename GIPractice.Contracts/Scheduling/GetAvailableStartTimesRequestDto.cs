using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public sealed record GetAvailableStartTimesRequestDto(
    [NotDefault] DateOnly Day,
    [NonZeroId] AppointmentTypeId AppointmentTypeId,
    [Range(5, 240)] int SlotStepMinutes);
