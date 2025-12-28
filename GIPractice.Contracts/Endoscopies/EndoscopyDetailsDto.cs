using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Endoscopies;

public sealed record EndoscopyDetailsDto(
    EndoscopyId Id,
    PatientId PatientId,
    EncounterId EncounterId,
    EndoscopyTypeId EndoscopyTypeId,
    DateTime StartUtc,
    DateTime? EndUtc,
    EndoscopyStatus Status,
    bool IsUrgent,
    string? Notes,
    byte[]? RowVersion);
