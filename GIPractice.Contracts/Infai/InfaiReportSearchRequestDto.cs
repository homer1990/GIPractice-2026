using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Infai;

public sealed record InfaiReportSearchRequestDto(
    [property: NonZeroId] PatientId? PatientId = null,
    [property: NonZeroId] EndoscopyId? EndoscopyId = null,
    InfaiReportStatus? Status = null,
    bool? IsUrgent = null,
    [property: NotDefault] DateOnly? SentFrom = null,
    [property: NotDefault] DateOnly? SentTo = null,
    PagedRequestDto? Paging = null);
