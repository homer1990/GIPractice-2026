using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Endoscopies;

public sealed record EndoscopySearchRequestDto(
    [param: NonZeroId] PatientId? PatientId = null,
    [param: NonZeroId] EncounterId? EncounterId = null,
    [param: NonZeroId] EndoscopyTypeId? EndoscopyTypeId = null,
    EndoscopyStatus? Status = null,

    [param: NotDefault] DateOnly? DateFrom = null,
    [param: NotDefault] DateOnly? DateTo = null,

    bool? IsUrgent = null,
    PagedRequestDto? Paging = null);
