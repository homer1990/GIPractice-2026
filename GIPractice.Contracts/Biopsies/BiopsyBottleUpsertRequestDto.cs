using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Biopsies;

public sealed record BiopsyBottleUpsertRequestDto(
    [param: NonZeroId] BiopsyBottleId? Id,

    [param: NonZeroId] PatientId PatientId,
    [param: NonZeroId] EndoscopyId EndoscopyId,

    [param: Required, MaxLength(32)] string LabelCode,
    [param: Required, MaxLength(200)] string SiteDescription,

    bool IsUrgent,
    [param: MaxLength(2000)] string? Notes,

    byte[]? RowVersion);
