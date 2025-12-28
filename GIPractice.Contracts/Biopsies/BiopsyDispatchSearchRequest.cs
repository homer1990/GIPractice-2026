using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Common.Validation;

namespace GIPractice.Contracts.Biopsies;

public sealed record BiopsyDispatchSearchRequestDto(
    [param: MaxLength(100)] string? ProtocolNumber = null,
    bool? IsClosed = null,
    [param: NotDefault] DateOnly? CreatedFrom = null,
    [param: NotDefault] DateOnly? CreatedTo = null,
    PagedRequestDto? Paging = null);
