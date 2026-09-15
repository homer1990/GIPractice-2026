namespace GIPractice.Server.Data;

// Internal persistence records. Encounter exists here because it is relational glue,
// not a user-facing clinical entity.
internal sealed class EncounterRow
{
    public Guid Id { get; init; }
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; init; }
    public DateTime StartedAtUtc { get; init; }
    public DateTime? EndedAtUtc { get; set; }
}

internal sealed class EndoscopyRow
{
    public Guid Id { get; init; }
    public Guid EncounterId { get; init; }
    public string TypeCode { get; set; } = null!;
    public string? ReportJson { get; set; }
}

internal sealed class ClinicalExamRow
{
    public Guid Id { get; init; }
    public Guid EncounterId { get; init; }
    public string TypeCode { get; set; } = null!;
    public string? Findings { get; set; }
}

internal sealed class PrescriptionRow
{
    public Guid Id { get; init; }
    public Guid EncounterId { get; init; }
    public string? Text { get; set; }
}

internal sealed class VisitRow
{
    public Guid Id { get; init; }
    public Guid EncounterId { get; init; }
    public string? Notes { get; set; }
}

internal sealed class InfaiTestRow
{
    public Guid Id { get; init; }
    public Guid EncounterId { get; init; }
    public short Result { get; set; }
    public bool PatientContacted { get; set; }
    public string? ReportStorageKey { get; set; }
}
