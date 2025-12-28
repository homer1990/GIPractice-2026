using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Encounters;

public sealed record EncounterUpsertRequestDto(
    EncounterId? Id,

    [NonZeroId] PatientId PatientId,
    [NonZeroId] EncounterTypeId EncounterTypeId,

    [NotDefault] DateTime StartUtc,
    DateTime? EndUtc,

    [EnumDataType(typeof(EncounterStatus))] EncounterStatus Status,
    bool IsUrgent,

    [MaxLength(2000)] string? Notes,
    byte[]? RowVersion);
