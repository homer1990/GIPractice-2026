using GIPractice.Contracts.Common;

namespace GIPractice.Contracts.Patients;

public sealed record PatientSearchRequestDto(
    string? FirstName = null,
    string? LastName = null,
    string? FathersName = null,
    string? PersonalNumber = null,
    string? Phone = null,
    string? Email = null,

    // tri-state filters: null = ignore, true = must have, false = must NOT have
    bool? HasHadCA = null,
    bool? HasHadIBD = null,
    bool? HasPendingBiopsies = null,
    bool? HasScheduledEndo = null,

    PagedRequestDto? Paging = null);
