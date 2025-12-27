namespace GIPractice.Contracts.Dispatch;

public sealed record BiopsyDispatchDetailsDto(
    BiopsyDispatchBundleDto Bundle,
    IReadOnlyList<BiopsyDispatchRowDto> Rows);
