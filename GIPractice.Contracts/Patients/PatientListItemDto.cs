using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Patients;

public sealed record PatientListItemDto(
    PatientId Id,
    string LastName,
    string FirstName,
    string? FathersName,
    DateTime? BirthDate,
    string? PersonalNumber,
    string? PhoneNumber,
    string? Email);
