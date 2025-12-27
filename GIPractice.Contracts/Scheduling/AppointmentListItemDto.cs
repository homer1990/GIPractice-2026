using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public sealed record AppointmentListItemDto(
    AppointmentId Id,
    PatientId PatientId,
    string PatientFullName,
    string? PatientPhoneNumber,

    DateTime StartUtc,
    int DurationMinutes,

    AppointmentTypeId AppointmentTypeId,
    string AppointmentTypeName,

    AppointmentStatus Status,
    bool IsUrgent,
    string? Notes,

    EncounterId? EncounterId,      // set when resolved
    byte[]? RowVersion);