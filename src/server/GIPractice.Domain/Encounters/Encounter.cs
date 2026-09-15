namespace GIPractice.Domain.Encounters;

public enum EncounterStatus
{
    Underway = 1,
    Completed = 2,
    Aborted = 3
}

public sealed class Encounter
{
    public EncounterId Id { get; }
    public PatientId PatientId { get; }
    public AppointmentId? AppointmentId { get; }
    public DateTimeOffset StartedAtUtc { get; }
    public DateTimeOffset? EndedAtUtc { get; private set; }
    public EncounterStatus Status { get; private set; }
    public bool RequiresExclusiveSlot { get; }
    public bool IsUrgent { get; private set; }
    public string? Notes { get; private set; }

    public bool OccupiesActiveSlot => RequiresExclusiveSlot && Status == EncounterStatus.Underway;

    public Encounter(
        EncounterId id,
        PatientId patientId,
        DateTimeOffset startedAtUtc,
        bool requiresExclusiveSlot,
        AppointmentId? appointmentId = null,
        bool isUrgent = false,
        string? notes = null)
    {
        Id = id;
        PatientId = patientId;
        AppointmentId = appointmentId;
        StartedAtUtc = startedAtUtc.ToUniversalTime();
        RequiresExclusiveSlot = requiresExclusiveSlot;
        IsUrgent = isUrgent;
        Notes = NormalizeNotes(notes);
        Status = EncounterStatus.Underway;
    }

    public void Complete(DateTimeOffset endedAtUtc)
    {
        EnsureUnderway();
        var utc = endedAtUtc.ToUniversalTime();
        if (utc < StartedAtUtc)
            throw new DomainRuleViolationException("Encounter end cannot precede its start.");

        EndedAtUtc = utc;
        Status = EncounterStatus.Completed;
    }

    public void Abort(DateTimeOffset endedAtUtc)
    {
        EnsureUnderway();
        var utc = endedAtUtc.ToUniversalTime();
        if (utc < StartedAtUtc)
            throw new DomainRuleViolationException("Encounter end cannot precede its start.");

        EndedAtUtc = utc;
        Status = EncounterStatus.Aborted;
    }

    public void SetUrgent(bool value) => IsUrgent = value;
    public void SetNotes(string? notes) => Notes = NormalizeNotes(notes);

    private void EnsureUnderway()
    {
        if (Status != EncounterStatus.Underway)
            throw new DomainRuleViolationException($"Encounter is already {Status}.");
    }

    private static string? NormalizeNotes(string? notes)
    {
        if (string.IsNullOrWhiteSpace(notes)) return null;
        var normalized = notes.Trim();
        if (normalized.Length > 8000) throw new ArgumentOutOfRangeException(nameof(notes));
        return normalized;
    }
}
