using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public sealed record PathologyParcelUpdateRequestDto(
    [param: NonZeroId] PathologyParcelId Id,

    DateTime? DispatchedAtUtc,

    [param: MaxLength(100)] string? CourierName,
    [param: MaxLength(100)] string? TrackingNumber,
    [param: MaxLength(500)] string? Notes,

    byte[]? RowVersion);