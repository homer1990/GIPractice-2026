using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Dispatch;

public sealed record BiopsyDispatchBundleDto(
    BiopsyDispatchBundleId Id,
    DateTime CreatedUtc,
    string ProtocolNumber,     // your “dispatch protocol” identifier
    string? Notes,
    bool IsClosed);
