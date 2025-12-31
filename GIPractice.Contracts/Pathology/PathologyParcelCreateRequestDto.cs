using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public sealed record PathologyParcelCreateRequestDto(
    [param: NonZeroId] PathologistId PathologistId,

    DateTime? DispatchedAtUtc = null,

    [param: MaxLength(100)] string? CourierName = null,
    [param: MaxLength(100)] string? TrackingNumber = null,
    [param: MaxLength(500)] string? Notes = null);