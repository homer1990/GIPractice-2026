using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Biopsies;

public sealed record BiopsyBottleSearchRequestDto(
    PatientId? PatientId = null,
    EndoscopyId? EndoscopyId = null,
    bool? IsUrgent = null,
    PagedRequestDto? Paging = null);
