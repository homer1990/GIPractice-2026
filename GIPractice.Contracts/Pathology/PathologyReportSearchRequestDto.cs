using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public sealed record PathologyReportSearchRequestDto(
    PatientId? PatientId = null,
    EndoscopyId? EndoscopyId = null,
    BiopsyDispatchBundleId? BiopsyDispatchBundleId = null,
    PathologyReportStatus? Status = null,
    bool? IsUrgent = null,
    DateOnly? SentFrom = null,
    DateOnly? SentTo = null,
    PagedRequestDto? Paging = null);
