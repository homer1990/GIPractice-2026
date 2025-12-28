using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Biopsies;

public sealed record BiopsyDispatchCloseRequestDto(
    BiopsyDispatchBundleId Id,
    byte[]? RowVersion);
