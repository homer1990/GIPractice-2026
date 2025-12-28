using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public sealed record PathologyReportSearchRequestDto(
    [property: NonZeroId] PatientId? PatientId = null,
    [property: NonZeroId] EndoscopyId? EndoscopyId = null,
    [property: NonZeroId] BiopsyDispatchBundleId? BiopsyDispatchBundleId = null,
    PathologyReportStatus? Status = null,
    bool? IsUrgent = null,
    [property: NotDefault] DateOnly? SentFrom = null,
    [property: NotDefault] DateOnly? SentTo = null,
    PagedRequestDto? Paging = null);
