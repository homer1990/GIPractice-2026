using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Endoscopies;

public sealed record EndoscopySearchRequestDto(
    [property: NonZeroId] PatientId? PatientId = null,
    [property: NonZeroId] EncounterId? EncounterId = null,
    [property: NonZeroId] EndoscopyTypeId? EndoscopyTypeId = null,
    EndoscopyStatus? Status = null,

    [property: NotDefault] DateOnly? DateFrom = null,
    [property: NotDefault] DateOnly? DateTo = null,

    bool? IsUrgent = null,
    PagedRequestDto? Paging = null);
