using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public sealed record PathologyParcelListItemDto(
    PathologyParcelId Id,

    PathologistId PathologistId,
    string ParcelCode,

    DateTime CreatedAtUtc,
    DateTime? DispatchedAtUtc,

    string MonetarySum,

    int NoOfBiopsyBottles,
    int NoOfEndoscopies
    );