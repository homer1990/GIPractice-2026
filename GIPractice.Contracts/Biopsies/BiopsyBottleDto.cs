using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Biopsies;

public sealed record BiopsyBottleDto(
    BiopsyBottleId Id,
    int PatientId,
    int EndoscopyId,

    string LabelCode,      // what you print/stick
    string SiteDescription,
    bool IsUrgent,
    string? Notes);
