using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public sealed record AppointmentUpsertRequestDto(
    AppointmentId? Id,
    PatientId PatientId,

    DateTime StartUtc,
    int DurationMinutes,

    AppointmentTypeId AppointmentTypeId,
    string AppointmentTypeName, // denormalized for display (optional but handy)

    bool IsUrgent,
    string? Notes,

    AppointmentStatus Status,
    byte[]? RowVersion);
