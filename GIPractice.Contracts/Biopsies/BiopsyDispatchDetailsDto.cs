namespace GIPractice.Contracts.Biopsies;

public sealed record BiopsyDispatchDetailsDto(
    BiopsyDispatchBundleDto Bundle,
    IReadOnlyList<BiopsyDispatchRowDto> Rows);
