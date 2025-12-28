using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Encounters;

public sealed record EncounterListItemDto(
    EncounterId Id,
    PatientId PatientId,
    EncounterTypeId EncounterTypeId,
    string EncounterTypeName,
    DateTime StartUtc,
    EncounterStatus Status,
    bool IsUrgent);
