namespace GIPractice.Domain;

public readonly record struct PatientId
{
    public Guid Value { get; }

    public PatientId(Guid value)
    {
        if (value == Guid.Empty) throw new ArgumentException("PatientId cannot be empty.", nameof(value));
        Value = value;
    }

    public static PatientId New() => new(Guid.CreateVersion7());
    public override string ToString() => Value.ToString();
}

public readonly record struct AppointmentId
{
    public Guid Value { get; }

    public AppointmentId(Guid value)
    {
        if (value == Guid.Empty) throw new ArgumentException("AppointmentId cannot be empty.", nameof(value));
        Value = value;
    }

    public static AppointmentId New() => new(Guid.CreateVersion7());
    public override string ToString() => Value.ToString();
}

public readonly record struct EncounterId
{
    public Guid Value { get; }

    public EncounterId(Guid value)
    {
        if (value == Guid.Empty) throw new ArgumentException("EncounterId cannot be empty.", nameof(value));
        Value = value;
    }

    public static EncounterId New() => new(Guid.CreateVersion7());
    public override string ToString() => Value.ToString();
}
