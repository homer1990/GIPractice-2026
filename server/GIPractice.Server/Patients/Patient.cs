namespace GIPractice.Server.Patients;

public sealed class Patient
{
    public Guid Id { get; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? FathersName { get; private set; }
    public DateOnly? BirthDate { get; private set; }

    public Patient(Guid id, string firstName, string lastName, string? fathersName = null, DateOnly? birthDate = null)
    {
        if (id == Guid.Empty) throw new ArgumentException("Patient ID is required.", nameof(id));
        Id = id;
        FirstName = Required(firstName, nameof(firstName));
        LastName = Required(lastName, nameof(lastName));
        FathersName = Optional(fathersName);
        BirthDate = birthDate;
    }

    public static Patient Create(string firstName, string lastName, string? fathersName = null, DateOnly? birthDate = null) =>
        new(Guid.CreateVersion7(), firstName, lastName, fathersName, birthDate);

    public void CorrectIdentity(string firstName, string lastName, string? fathersName, DateOnly? birthDate)
    {
        FirstName = Required(firstName, nameof(firstName));
        LastName = Required(lastName, nameof(lastName));
        FathersName = Optional(fathersName);
        BirthDate = birthDate;
    }

    private static string Required(string value, string parameterName)
    {
        var normalized = value.Trim();
        if (normalized.Length == 0) throw new ArgumentException("Value is required.", parameterName);
        if (normalized.Length > 200) throw new ArgumentOutOfRangeException(parameterName);
        return normalized;
    }

    private static string? Optional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalized = value.Trim();
        if (normalized.Length > 200) throw new ArgumentOutOfRangeException(nameof(value));
        return normalized;
    }
}
