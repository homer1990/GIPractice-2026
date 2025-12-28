using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Endoscopies;

public sealed record EndoscopyListItemDto(
    EndoscopyId Id,
    PatientId PatientId,
    EncounterId EncounterId,
    EndoscopyTypeId EndoscopyTypeId,
    string EndoscopyTypeName,
    DateTime StartUtc,
    EndoscopyStatus Status,
    bool IsUrgent);
