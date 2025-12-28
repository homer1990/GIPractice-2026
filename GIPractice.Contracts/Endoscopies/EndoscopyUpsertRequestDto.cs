using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Endoscopies;

public sealed record EndoscopyUpsertRequestDto(
    [property: NonZeroId] EndoscopyId? Id,

    [property: NonZeroId] PatientId PatientId,
    [property: NonZeroId] EncounterId EncounterId,
    [property: NonZeroId] EndoscopyTypeId EndoscopyTypeId,

    [property: NotDefault] DateTime StartUtc,
    [property: NotDefault] DateTime? EndUtc,

    [property: EnumDataType(typeof(EndoscopyStatus))] EndoscopyStatus Status,
    bool IsUrgent,

    [property: MaxLength(4000)] string? Notes,
    byte[]? RowVersion);
