using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Patients;

public sealed record PatientUpsertRequestDto(
    [property: NonZeroId] PatientId? Id,

    [property: Required, MaxLength(100)] string LastName,
    [property: Required, MaxLength(100)] string FirstName,
    [property: MaxLength(100)] string? FathersName,

    DateTime? BirthDate,
    [property: RegularExpression(@"^\d{12}$")] string? PersonalNumber,
    [property: MaxLength(30)] string? Gender,

    [property: MaxLength(30)] string? PhoneNumber,
    [property: EmailAddress, MaxLength(254)] string? Email,
    [property: MaxLength(250)] string? Address,

    bool HasHadCA,
    bool HasHadIBD,
    bool HasPendingBiopsies,
    bool HasScheduledEndo,

    byte[]? PhotoBytes,
    [property: MaxLength(100)] string? PhotoContentType,

    byte[]? RowVersion);
