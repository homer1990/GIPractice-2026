using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public sealed record AppointmentUpsertRequestDto(
    AppointmentId? Id,

    [NonZeroId] PatientId PatientId,

    [NotDefault] DateTime StartUtc,
    [Range(5, 24 * 60)] int DurationMinutes,

    [NonZeroId] AppointmentTypeId AppointmentTypeId,

    // denormalized for display
    [MaxLength(120)] string AppointmentTypeName,

    bool IsUrgent,
    [MaxLength(2000)] string? Notes,

    [EnumDataType(typeof(AppointmentStatus))] AppointmentStatus Status,
    byte[]? RowVersion);
