namespace GIPractice.Domain.Patients;

public enum PatientGender
{
    Unspecified = 0,
    Male = 1,
    Female = 2,
    Other = 3
}

public sealed class Patient
{
    public PatientId Id { get; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? FathersName { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public PatientGender Gender { get; private set; }

    public string FullName => $"{FirstName} {LastName}".Trim();

    public Patient(
        PatientId id,
        string firstName,
        string lastName,
        string? fathersName = null,
        DateOnly? birthDate = null,
        PatientGender gender = PatientGender.Unspecified)
    {
        Id = id;
        FirstName = RequiredName(firstName, nameof(firstName));
        LastName = RequiredName(lastName, nameof(lastName));
        FathersName = OptionalName(fathersName);
        BirthDate = birthDate;
        Gender = gender;
    }

    public void Rename(string firstName, string lastName, string? fathersName)
    {
        FirstName = RequiredName(firstName, nameof(firstName));
        LastName = RequiredName(lastName, nameof(lastName));
        FathersName = OptionalName(fathersName);
    }

    public void SetDemographics(DateOnly? birthDate, PatientGender gender)
    {
        BirthDate = birthDate;
        Gender = gender;
    }

    private static string RequiredName(string value, string parameterName)
    {
        var normalized = value.Trim();
        if (normalized.Length == 0) throw new ArgumentException("Name cannot be empty.", parameterName);
        if (normalized.Length > 200) throw new ArgumentOutOfRangeException(parameterName, "Name cannot exceed 200 characters.");
        return normalized;
    }

    private static string? OptionalName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalized = value.Trim();
        if (normalized.Length > 200) throw new ArgumentOutOfRangeException(nameof(value), "Name cannot exceed 200 characters.");
        return normalized;
    }
}
