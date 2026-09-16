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

internal sealed class ClinicalExamRow
{
    public Guid Id { get; init; }
    public Guid EncounterId { get; init; }
    public string ExaminationText { get; set; } = string.Empty;
    public string? Assessment { get; set; }
}

internal sealed class ClinicalExamAnnotationRow
{
    public Guid Id { get; init; }
    public Guid ClinicalExamId { get; init; }
    public int Start { get; set; }
    public int Length { get; set; }
    public short Kind { get; set; }
    public string? CodeSystem { get; set; }
    public string? Code { get; set; }
    public Guid? TargetPatientId { get; set; }
    public bool ConfirmedByUser { get; set; }
}

internal sealed class EndoscopyRow
{
    public Guid Id { get; init; }
    public Guid EncounterId { get; init; }
    public string TypeCode { get; set; } = null!;
    public DateTime StartedAtUtc { get; init; }
    public DateTime? EndedAtUtc { get; set; }
    public string? Indication { get; set; }
    public short Outcome { get; set; }
    public string? ExtentReachedCode { get; set; }
    public short PreparationMode { get; set; }
    public string? PreparationQualityCode { get; set; }
    public short SedationMode { get; set; }
    public string? Impression { get; set; }
    public string? Recommendations { get; set; }
}

internal sealed class EndoscopyFindingRow
{
    public Guid Id { get; init; }
    public Guid EndoscopyId { get; init; }
    public string AnatomicalSiteCode { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? FindingCode { get; set; }
    public string? SeverityCode { get; set; }
    public decimal? SizeMm { get; set; }
}

internal sealed class EndoscopyTerminationReasonRow
{
    public Guid Id { get; init; }
    public Guid EndoscopyId { get; init; }
    public short Kind { get; set; }
    public string? Description { get; set; }
    public Guid? RelatedFindingId { get; set; }
}

internal sealed class EndoscopyEventRow
{
    public Guid Id { get; init; }
    public Guid EndoscopyId { get; init; }
    public DateTime OccurredAtUtc { get; set; }
    public short Phase { get; set; }
    public string EventCode { get; set; } = null!;
    public string? Description { get; set; }
}

internal sealed class EndoscopySpecimenRow
{
    public Guid Id { get; init; }
    public Guid EndoscopyId { get; init; }
    public string AnatomicalSiteCode { get; set; } = null!;
    public short Priority { get; set; }
    public string? Description { get; set; }
    public string? PriorityReason { get; set; }
    public Guid? RelatedFindingId { get; set; }
}

internal sealed class ClinicalMediaRow
{
    public Guid Id { get; init; }
    public Guid EndoscopyId { get; init; }
    public short Kind { get; set; }
    public short Variant { get; set; }
    public string StorageKey { get; set; } = null!;
    public string MimeType { get; set; } = null!;
    public string Sha256 { get; set; } = null!;
    public int Width { get; set; }
    public int Height { get; set; }
    public DateTime CapturedAtUtc { get; set; }
    public string? Codec { get; set; }
    public string? Container { get; set; }
    public int? BitDepth { get; set; }
    public long? DurationMilliseconds { get; set; }
    public Guid? FindingId { get; set; }
    public Guid? DerivedFromMediaId { get; set; }
    public string? Caption { get; set; }
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

internal sealed class PatientHistoryEntryRow
{
    public Guid Id { get; init; }
    public Guid PatientId { get; init; }
    public short Kind { get; set; }
    public string Text { get; set; } = null!;
    public DateTime? Date { get; set; }
    public bool? Active { get; set; }
    public string? CodeSystem { get; set; }
    public string? Code { get; set; }
    public string? Notes { get; set; }
}
