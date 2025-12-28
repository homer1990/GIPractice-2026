using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public sealed record PathologyReportSearchRequestDto(
    [param: NonZeroId] PatientId? PatientId = null,
    [param: NonZeroId] EndoscopyId? EndoscopyId = null,
    [param: NonZeroId] BiopsyDispatchBundleId? BiopsyDispatchBundleId = null,
    PathologyReportStatus? Status = null,
    bool? IsUrgent = null,
    [param: NotDefault] DateOnly? SentFrom = null,
    [param: NotDefault] DateOnly? SentTo = null,
    PagedRequestDto? Paging = null);
