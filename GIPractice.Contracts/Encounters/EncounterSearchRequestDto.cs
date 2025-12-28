using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Encounters;

public sealed record EncounterSearchRequestDto(
    PatientId? PatientId = null,
    EncounterTypeId? EncounterTypeId = null,
    EncounterStatus? Status = null,
    DateOnly? DateFrom = null,
    DateOnly? DateTo = null,
    bool ? IsUrgent = null,
    PagedRequestDto? Paging = null);
