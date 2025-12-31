using GIPractice.Contracts.Common;

namespace GIPractice.Contracts.Pathologists;

public sealed record PathologistSearchRequestDto(
    [param: MaxLength(200)] string? Name = null,
    PagedRequestDto? Paging = null);