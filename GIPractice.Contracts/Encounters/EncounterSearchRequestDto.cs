using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Encounters;

public sealed record EncounterSearchRequestDto(
    [param: NonZeroId] PatientId? PatientId = null,
    [param: NonZeroId] EncounterTypeId? EncounterTypeId = null,
    EncounterStatus? Status = null,

    [param: NotDefault] DateOnly? DateFrom = null,
    [param: NotDefault] DateOnly? DateTo = null,

    bool? IsUrgent = null,
    PagedRequestDto? Paging = null);
