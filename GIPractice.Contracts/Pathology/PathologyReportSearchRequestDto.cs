using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public sealed record PathologyReportSearchRequestDto(
    [param: NonZeroId] PatientId? PatientId = null,
    [param: NonZeroId] EndoscopyId? EndoscopyId = null,
    [param: NonZeroId] PathologistId? PathologistId = null,

    [param: MaxLength(32)] string? DispatchParcelCode = null,

    PathologyReportStatus? Status = null,
    bool? IsUrgent = null,

    [param: NotDefault] DateTime? SentFromUtc = null,
    [param: NotDefault] DateTime? SentToUtc = null,
    [param: NotDefault] DateTime? ReceivedFromUtc = null,
    [param: NotDefault] DateTime? ReceivedToUtc = null,

    PagedRequestDto? Paging = null);