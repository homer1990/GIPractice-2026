using GIPractice.Contracts.Common;

namespace GIPractice.Contracts.Biopsies;

public sealed record BiopsyDispatchSearchRequestDto(
    string? ProtocolNumber = null,
    bool? IsClosed = null,
    DateOnly? CreatedFrom = null,
    DateOnly? CreatedTo = null,
    PagedRequestDto? Paging = null);
