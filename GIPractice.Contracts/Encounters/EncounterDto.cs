using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Encounters;

public sealed record EncounterDto(
    EncounterId Id,
    PatientId PatientId,
    AppointmentId? AppointmentId,

    DateTime Start,
    DateTime? End,

    string EncounterTypeName,   // e.g. "Ιατρείο", "Ενδοσκόπηση", "INFAI"
    string? EndoscopyTypeName,  // if encounter is endoscopy

    EncounterStatus Status,
    string? Notes);
