using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public sealed record PathologyParcelCreateRequestDto(
    [param: NonZeroId] PathologistId PathologistId,

    // Option 1 (final): a parcel/protocol is created from endoscopies that produced biopsies.
    // Each EndoscopyId must exist and must have at least 1 BiopsyBottle.
    [param: Required, MinLength(1)] EndoscopyId[] EndoscopyIds,

    DateTime? DispatchedAtUtc = null,

    [param: MaxLength(100)] string? CourierName = null,
    [param: MaxLength(100)] string? TrackingNumber = null,
    [param: MaxLength(500)] string? Notes = null);
