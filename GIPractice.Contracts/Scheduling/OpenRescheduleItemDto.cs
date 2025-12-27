using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public sealed record OpenRescheduleItemDto(
    AppointmentId Id,
    PatientId PatientId,
    string PatientFullName,
    string? PatientPhoneNumber,
    string AppointmentTypeName,
    string? Notes,
    DateTime CreatedUtc,
    DateTime? NextContactUtc);
