using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Encounters;

public sealed record EncounterSearchRequestDto(
    [property: NonZeroId] PatientId? PatientId = null,
    [property: NonZeroId] EncounterTypeId? EncounterTypeId = null,
    EncounterStatus? Status = null,

    [property: NotDefault] DateOnly? DateFrom = null,
    [property: NotDefault] DateOnly? DateTo = null,

    bool? IsUrgent = null,
    PagedRequestDto? Paging = null);
