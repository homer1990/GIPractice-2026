using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Biopsies;

public sealed record BiopsyBottleUpsertRequestDto(
    BiopsyBottleId? Id,
    PatientId PatientId,
    EndoscopyId EndoscopyId,

    string LabelCode,
    string SiteDescription,
    bool IsUrgent,
    string? Notes,

    byte[]? RowVersion);
