using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Patients;

public sealed record PatientDetailsDto(
    PatientId Id,

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

    // Optional photo contract (keep simple for now)
    // Later you can switch to PhotoId + separate endpoint, but this is fine for scaffolding.
    byte[]? PhotoBytes,
    string? PhotoContentType,

    // Optimistic concurrency (optional; Base64 in JSON)
    byte[]? RowVersion);
