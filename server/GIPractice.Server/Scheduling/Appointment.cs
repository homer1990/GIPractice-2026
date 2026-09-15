namespace GIPractice.Server.Scheduling;

public enum AppointmentStatus
{
    Scheduled = 1,
    Arrived = 2,
    Cancelled = 3,
    NoShow = 4,
    Completed = 5
}

public sealed class Appointment
{
    public Guid Id { get; }
    public Guid PatientId { get; private set; }
    public string TypeCode { get; private set; }
    public DateTimeOffset StartUtc { get; private set; }
    public int DurationMinutes { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public string? Notes { get; private set; }

    public Appointment(
        Guid id,
        Guid patientId,
        string typeCode,
        DateTimeOffset startUtc,
        int durationMinutes,
        AppointmentStatus status = AppointmentStatus.Scheduled,
        string? notes = null)
    {
        if (id == Guid.Empty) throw new ArgumentException("Appointment ID is required.", nameof(id));
        if (patientId == Guid.Empty) throw new ArgumentException("Patient ID is required.", nameof(patientId));

        Id = id;
        PatientId = patientId;
        TypeCode = NormalizeType(typeCode);
        StartUtc = startUtc.ToUniversalTime();
        DurationMinutes = ValidateDuration(durationMinutes);
        Status = status;
        Notes = NormalizeNotes(notes);
    }

    public static Appointment Create(
        Guid patientId,
        string typeCode,
        DateTimeOffset startUtc,
        int durationMinutes,
        string? notes = null) =>
        new(Guid.CreateVersion7(), patientId, typeCode, startUtc, durationMinutes, notes: notes);

    public void CorrectPlanning(
        Guid patientId,
        string typeCode,
        DateTimeOffset startUtc,
        int durationMinutes,
        string? notes)
    {
        if (patientId == Guid.Empty) throw new ArgumentException("Patient ID is required.", nameof(patientId));

        PatientId = patientId;
        TypeCode = NormalizeType(typeCode);
        StartUtc = startUtc.ToUniversalTime();
        DurationMinutes = ValidateDuration(durationMinutes);
        Notes = NormalizeNotes(notes);
    }

    public void MarkArrived()
    {
        if (Status == AppointmentStatus.Arrived) return;
        if (Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException($"Appointment cannot arrive from {Status}.");
        Status = AppointmentStatus.Arrived;
    }

    public void Cancel() => Status = AppointmentStatus.Cancelled;
    public void MarkNoShow() => Status = AppointmentStatus.NoShow;
    public void Complete() => Status = AppointmentStatus.Completed;

    private static string NormalizeType(string value)
    {
        var normalized = value.Trim();
        if (normalized.Length == 0) throw new ArgumentException("Appointment type is required.", nameof(value));
        if (normalized.Length > 64) throw new ArgumentOutOfRangeException(nameof(value));
        return normalized;
    }

    private static int ValidateDuration(int value)
    {
        if (value is <= 0 or > 24 * 60) throw new ArgumentOutOfRangeException(nameof(value));
        return value;
    }

    private static string? NormalizeNotes(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalized = value.Trim();
        if (normalized.Length > 4000) throw new ArgumentOutOfRangeException(nameof(value));
        return normalized;
    }
}
