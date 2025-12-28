using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Common.Validation;

namespace GIPractice.Contracts.Biopsies;

public sealed record BiopsyDispatchSearchRequestDto(
    [property: MaxLength(100)] string? ProtocolNumber = null,
    bool? IsClosed = null,
    [property: NotDefault] DateOnly? CreatedFrom = null,
    [property: NotDefault] DateOnly? CreatedTo = null,
    PagedRequestDto? Paging = null);
