using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Biopsies;

public sealed record BiopsyBottleSearchRequestDto(
    [property: NonZeroId] PatientId? PatientId = null,
    [property: NonZeroId] EndoscopyId? EndoscopyId = null,
    bool? IsUrgent = null,
    PagedRequestDto? Paging = null);
