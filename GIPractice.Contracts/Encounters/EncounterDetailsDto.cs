using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Encounters;

public sealed record EncounterDetailsDto(
    EncounterId Id,
    PatientId PatientId,
    EncounterTypeId EncounterTypeId,
    DateTime StartUtc,
    DateTime? EndUtc,
    EncounterStatus Status,
    bool IsUrgent,
    string? Notes,
    byte[]? RowVersion);
