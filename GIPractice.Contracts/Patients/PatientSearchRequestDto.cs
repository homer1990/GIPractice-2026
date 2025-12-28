using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common;

namespace GIPractice.Contracts.Patients;

public sealed record PatientSearchRequestDto(
    [param: MaxLength(100)] string? FirstName = null,
    [param: MaxLength(100)] string? LastName = null,
    [param: MaxLength(100)] string? FathersName = null,

    // 12-digit Greek Personal Number (Προσωπικός Αριθμός)
    [param: RegularExpression(@"^\d{12}$")] string? PersonalNumber = null,

    [param: MaxLength(30)] string? Phone = null,
    [param: EmailAddress, MaxLength(254)] string? Email = null,

    // tri-state filters: null = ignore, true = must have, false = must NOT have
    bool? HasHadCA = null,
    bool? HasHadIBD = null,
    bool? HasPendingBiopsies = null,
    bool? HasScheduledEndo = null,

    PagedRequestDto? Paging = null);
