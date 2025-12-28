using System.ComponentModel.DataAnnotations;

namespace GIPractice.Contracts.Biopsies;

public sealed record BiopsyDispatchCreateRequestDto(
    [property: Required, MaxLength(100)] string ProtocolNumber,
    [property: MaxLength(2000)] string? Notes = null);
