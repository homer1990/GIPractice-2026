using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Endoscopies;

public sealed record EndoscopyUpsertRequestDto(
    EndoscopyId? Id,
    PatientId PatientId,
    EncounterId EncounterId,

    EndoscopyTypeId EndoscopyTypeId,
    string EndoscopyTypeName,

    DateTime StartUtc,
    DateTime? EndUtc,

    bool IsUrgent,
    string? Notes,
    string? ReportText,

    byte[]? RowVersion);