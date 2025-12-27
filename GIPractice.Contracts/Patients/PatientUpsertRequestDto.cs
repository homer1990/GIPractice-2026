using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Patients;

public sealed record PatientUpsertRequestDto(
    PatientId? Id,

    string LastName,
    string FirstName,
    string? FathersName,

    DateTime? BirthDate,
    string? PersonalNumber,
    string? Gender,

    string? PhoneNumber,
    string? Email,
    string? Address,

    bool HasHadCA,
    bool HasHadIBD,
    bool HasPendingBiopsies,
    bool HasScheduledEndo,

    byte[]? PhotoBytes,
    string? PhotoContentType,

    byte[]? RowVersion);
