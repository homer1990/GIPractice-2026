using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Biopsies;

public sealed record BiopsyBottleUpsertRequestDto(
    [property: NonZeroId] BiopsyBottleId? Id,

    [property: NonZeroId] PatientId PatientId,
    [property: NonZeroId] EndoscopyId EndoscopyId,

    [property: Required, MaxLength(32)] string LabelCode,
    [property: Required, MaxLength(200)] string SiteDescription,

    bool IsUrgent,
    [property: MaxLength(2000)] string? Notes,

    byte[]? RowVersion);
