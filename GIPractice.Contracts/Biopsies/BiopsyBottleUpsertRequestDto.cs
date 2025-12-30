using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Biopsies;

/// <summary>
/// Create/update request for a biopsy bottle.
/// 
/// Note: SiteDescription is derived from OrganAreas and is returned in <see cref="BiopsyBottleDto"/>.
/// Clients should send OrganAreaCodes (best-effort) instead of a free-form SiteDescription string.
/// </summary>
public sealed record BiopsyBottleUpsertRequestDto(
    [param: NonZeroId] BiopsyBottleId? Id,

    [param: NonZeroId] PatientId PatientId,
    [param: NonZeroId] EndoscopyId EndoscopyId,

    [param: Required, MaxLength(32)] string LabelCode,

    bool IsUrgent,
    [param: MaxLength(2000)] string? Notes,

    /// <summary>Optional organ-area codes (e.g. "GEJ", "ANTRUM").</summary>
    string[]? OrganAreaCodes = null,

    byte[]? RowVersion = null);
