namespace GIPractice.Server.Patients;

public enum PatientHistoryKind
{
    Diagnosis = 1,
    Surgery = 2,
    Medication = 3,
    Allergy = 4,
    FamilyHistory = 5,
    SocialHistory = 6,
    Other = 99
}

public sealed record PatientHistoryEntry(
    Guid Id,
    Guid PatientId,
    PatientHistoryKind Kind,
    string Text,
    DateOnly? Date = null,
    bool? Active = null,
    string? CodeSystem = null,
    string? Code = null,
    string? Notes = null);
