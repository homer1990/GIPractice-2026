using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Biopsies;

public sealed record BiopsyDispatchBundleDto(
    BiopsyDispatchBundleId Id,
    DateTime CreatedUtc,
    string ProtocolNumber,
    string? Notes,
    bool IsClosed,

    byte[]? RowVersion);
