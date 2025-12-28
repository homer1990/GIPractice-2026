using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Biopsies;

public sealed record BiopsyBottleSearchRequestDto(
    [param: NonZeroId] PatientId? PatientId = null,
    [param: NonZeroId] EndoscopyId? EndoscopyId = null,
    bool? IsUrgent = null,
    PagedRequestDto? Paging = null);
