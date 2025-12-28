using System.ComponentModel.DataAnnotations;

namespace GIPractice.Contracts.Biopsies;

public sealed record BiopsyDispatchCreateRequestDto(
    [param: Required, MaxLength(100)] string ProtocolNumber,
    [param: MaxLength(2000)] string? Notes = null);
