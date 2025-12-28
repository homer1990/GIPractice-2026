using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Patients;

public sealed record PatientUpsertRequestDto(
    [param: NonZeroId] PatientId? Id,

    [param: Required, MaxLength(100)] string LastName,
    [param: Required, MaxLength(100)] string FirstName,
    [param: MaxLength(100)] string? FathersName,

    DateTime? BirthDate,
    [param: RegularExpression(@"^\d{12}$")] string? PersonalNumber,
    [param: MaxLength(30)] string? Gender,

    [param: MaxLength(30)] string? PhoneNumber,
    [param: EmailAddress, MaxLength(254)] string? Email,
    [param: MaxLength(250)] string? Address,

    bool HasHadCA,
    bool HasHadIBD,
    bool HasPendingBiopsies,
    bool HasScheduledEndo,

    byte[]? PhotoBytes,
    [param: MaxLength(100)] string? PhotoContentType,

    byte[]? RowVersion);
