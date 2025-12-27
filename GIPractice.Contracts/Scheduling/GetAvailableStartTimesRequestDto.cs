using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public sealed record GetAvailableStartTimesRequestDto(
    DateOnly Day,
    AppointmentTypeId AppointmentTypeId,
    int SlotStepMinutes); // e.g. 30
