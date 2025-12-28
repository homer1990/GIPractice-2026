using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Endoscopies;

public sealed record EndoscopySearchRequestDto(
    PatientId? PatientId = null,
    EncounterId? EncounterId = null,
    EndoscopyTypeId? EndoscopyTypeId = null,
    EndoscopyStatus? Status = null,
    DateOnly? DateFrom = null,
    DateOnly? DateTo = null,
    bool? IsUrgent = null,
    PagedRequestDto? Paging = null);
