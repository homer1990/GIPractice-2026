using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Endoscopies;

public sealed record EndoscopyDetailsDto(
    EndoscopyId Id,
    PatientId PatientId,
    EncounterId EncounterId,

    EndoscopyTypeId EndoscopyTypeId,
    string EndoscopyTypeName,

    DateTime StartUtc,
    DateTime? EndUtc,

    bool IsUrgent,
    string? Notes,

    // Computed report fields can be separate, but keep placeholders here
    string? ReportText,

    byte[]? RowVersion);
