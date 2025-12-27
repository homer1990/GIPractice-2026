using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Encounters;

public sealed record EncounterListItemDto(
    EncounterId Id,
    PatientId PatientId,
    AppointmentId? AppointmentId,

    DateTime StartUtc,
    DateTime? EndUtc,

    int EncounterTypeId,
    string EncounterTypeName,

    string? EndoscopyTypeName,
    EncounterStatus Status,
    string? Notes);
