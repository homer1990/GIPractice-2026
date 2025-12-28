using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Endoscopies;

public sealed record EndoscopyUpsertRequestDto(
    [param: NonZeroId] EndoscopyId? Id,

    [param: NonZeroId] PatientId PatientId,
    [param: NonZeroId] EncounterId EncounterId,
    [param: NonZeroId] EndoscopyTypeId EndoscopyTypeId,

    [param: NotDefault] DateTime StartUtc,
    [param: NotDefault] DateTime? EndUtc,

    [param: EnumDataType(typeof(EndoscopyStatus))] EndoscopyStatus Status,
    bool IsUrgent,

    [param: MaxLength(4000)] string? Notes,
    byte[]? RowVersion);
