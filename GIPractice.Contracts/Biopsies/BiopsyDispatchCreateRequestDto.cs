namespace GIPractice.Contracts.Biopsies;

public sealed record BiopsyDispatchCreateRequestDto(
    string ProtocolNumber,
    string? Notes = null);
