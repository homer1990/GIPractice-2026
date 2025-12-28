using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Infai;

public sealed record InfaiReportSearchRequestDto(
    PatientId? PatientId = null,
    EndoscopyId? EndoscopyId = null,
    InfaiReportStatus? Status = null,
    bool? IsUrgent = null,
    DateOnly? SentFrom = null,
    DateOnly? SentTo = null,
    PagedRequestDto? Paging = null);
