using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Encounters;

public sealed record EncounterUpsertRequestDto(
    EncounterId? Id,
    PatientId PatientId,
    AppointmentId? AppointmentId,

    DateTime StartUtc,
    DateTime? EndUtc,

    int EncounterTypeId,
    string EncounterTypeName,
    string? Notes,

    byte[]? RowVersion);