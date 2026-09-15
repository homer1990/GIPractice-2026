namespace GIPractice.Domain.Scheduling;

public enum AppointmentKind
{
    Visit = 1,
    Endoscopy = 2,
    ClinicalExam = 3,
    Infai = 4
}

public enum AppointmentStatus
{
    Scheduled = 1,
    Arrived = 2,
    Resolved = 3,
    Cancelled = 4,
    NoShow = 5
}

public sealed class Appointment
{
    public AppointmentId Id { get; }
    public PatientId PatientId { get; }
    public AppointmentKind Kind { get; private set; }
    public DateTimeOffset ScheduledStartUtc { get; private set; }
    public int DurationMinutes { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public bool IsUrgent { get; private set; }
    public string? Notes { get; private set; }

    public Appointment(
        AppointmentId id,
        PatientId patientId,
        AppointmentKind kind,
        DateTimeOffset scheduledStartUtc,
        int durationMinutes,
        bool isUrgent = false,
        string? notes = null)
    {
        if (durationMinutes <= 0) throw new ArgumentOutOfRangeException(nameof(durationMinutes));

        Id = id;
        PatientId = patientId;
        Kind = kind;
        ScheduledStartUtc = scheduledStartUtc.ToUniversalTime();
        DurationMinutes = durationMinutes;
        IsUrgent = isUrgent;
        Notes = NormalizeNotes(notes);
        Status = AppointmentStatus.Scheduled;
    }

    public void Reschedule(DateTimeOffset scheduledStartUtc, int durationMinutes)
    {
        EnsureMutable();
        if (durationMinutes <= 0) throw new ArgumentOutOfRangeException(nameof(durationMinutes));
        ScheduledStartUtc = scheduledStartUtc.ToUniversalTime();
        DurationMinutes = durationMinutes;
    }

    public void MarkArrived()
    {
        if (Status != AppointmentStatus.Scheduled)
            throw new DomainRuleViolationException($"Only a scheduled appointment can arrive; current status is {Status}.");
        Status = AppointmentStatus.Arrived;
    }

    public void MarkResolved()
    {
        if (Status is not (AppointmentStatus.Scheduled or AppointmentStatus.Arrived))
            throw new DomainRuleViolationException($"Appointment cannot resolve from {Status}.");
        Status = AppointmentStatus.Resolved;
    }

    public void Cancel()
    {
        if (Status == AppointmentStatus.Resolved)
            throw new DomainRuleViolationException("A resolved appointment cannot be cancelled.");
        if (Status == AppointmentStatus.Cancelled) return;
        Status = AppointmentStatus.Cancelled;
    }

    public void MarkNoShow()
    {
        if (Status != AppointmentStatus.Scheduled)
            throw new DomainRuleViolationException($"Appointment cannot become no-show from {Status}.");
        Status = AppointmentStatus.NoShow;
    }

    public void SetUrgent(bool value) => IsUrgent = value;
    public void SetNotes(string? notes) => Notes = NormalizeNotes(notes);

    private void EnsureMutable()
    {
        if (Status is AppointmentStatus.Resolved or AppointmentStatus.Cancelled or AppointmentStatus.NoShow)
            throw new DomainRuleViolationException($"Appointment in status {Status} cannot be rescheduled.");
    }

    private static string? NormalizeNotes(string? notes)
    {
        if (string.IsNullOrWhiteSpace(notes)) return null;
        var normalized = notes.Trim();
        if (normalized.Length > 4000) throw new ArgumentOutOfRangeException(nameof(notes));
        return normalized;
    }
}
