using GIPractice.Contracts.Ids;

public sealed record PathologyParcelDto(
    PathologyParcelId Id,
    PathologistId PathologistId,
    string ParcelCode,
    DateTime CreatedAtUtc,
    DateTime? DispatchedAtUtc,
    string? CourierName,
    string? TrackingNumber,
    string? Notes,
    int ReportsCount,
    bool HasUrgent,
    byte[]? RowVersion);
