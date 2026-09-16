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
    public string? HistoryText { get; set; }
}

internal sealed class AnatomicalSiteRow
{
    public Guid Id { get; init; }
    public string Code { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public short Kind { get; set; }
    public Guid? ParentId { get; set; }
    public int SortOrder { get; set; }
}

internal sealed class AnatomicalSiteAliasRow
{
    public Guid Id { get; init; }
    public Guid AnatomicalSiteId { get; init; }
    public string Alias { get; set; } = null!;
}

internal sealed class ObservedLandmarkRow
{
    public Guid Id { get; init; }
    public Guid EndoscopyId { get; init; }
    public Guid AnatomicalSiteId { get; init; }
    public decimal Position { get; set; }
    public string UnitCode { get; set; } = null!;
    public short ReferencePoint { get; set; }
    public string? Note { get; set; }
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
    public short Priority { get; set; }
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

internal sealed class PathologyCaseRow
{
    public Guid Id { get; init; }
    public Guid EndoscopyId { get; init; }
    public DateTime CreatedAtUtc { get; set; }
    public bool IsUrgent { get; set; }
    public bool ReceiptRequested { get; set; }
    public short FeeWaiverReason { get; set; }
}

internal sealed class BiopsyContainerRow
{
    public Guid Id { get; init; }
    public Guid PathologyCaseId { get; init; }
    public string LabelCode { get; set; } = null!;
    public int Ordinal { get; set; }
    public string CollectionSiteText { get; set; } = null!;
    public DateTime CollectedAtUtc { get; set; }
    public string? Description { get; set; }
    public DateTime? ExternalReleasedAtUtc { get; set; }
    public string? ExternalReleaseNote { get; set; }
}

internal sealed class BiopsyContainerSiteRow
{
    public Guid BiopsyContainerId { get; init; }
    public Guid AnatomicalSiteId { get; init; }
}

internal sealed class PathologistRow
{
    public Guid Id { get; init; }
    public string DisplayName { get; set; } = null!;
    public string? Notes { get; set; }
}

internal sealed class ParcelRow
{
    public Guid Id { get; init; }
    public string ParcelNumber { get; set; } = null!;
    public Guid PathologistId { get; init; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? SentAtUtc { get; set; }
    public string? CourierName { get; set; }
    public string? TrackingNumber { get; set; }
    public decimal? CourierCost { get; set; }
    public string Currency { get; set; } = "EUR";
}

internal sealed class ParcelContainerRow
{
    public Guid ParcelId { get; init; }
    public Guid BiopsyContainerId { get; init; }
}

internal sealed class PathologyAssayRow
{
    public Guid Id { get; init; }
    public Guid PathologyCaseId { get; init; }
    public string AssayCode { get; set; } = null!;
    public DateTime RequestedAtUtc { get; set; }
    public short Status { get; set; }
    public Guid? BiopsyContainerId { get; set; }
    public string? Description { get; set; }
}

internal sealed class PathologyChargeRow
{
    public Guid Id { get; init; }
    public Guid PathologyCaseId { get; init; }
    public short Kind { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public decimal CalculatedAmount { get; set; }
    public decimal ChargedAmount { get; set; }
    public string Currency { get; set; } = "EUR";
    public short WaiverReason { get; set; }
    public int? ContainerCountSnapshot { get; set; }
    public string? PricingPolicyCode { get; set; }
    public Guid? BiopsyContainerId { get; set; }
    public Guid? AssayId { get; set; }
    public Guid? BilledInParcelId { get; set; }
    public string? Description { get; set; }
}

internal sealed class PathologyDocumentAssetRow
{
    public Guid Id { get; init; }
    public string Sha256 { get; set; } = null!;
    public string MimeType { get; set; } = null!;
    public string StorageKey { get; set; } = null!;
    public long ByteLength { get; set; }
}

internal sealed class PathologyReportTemplateRow
{
    public Guid Id { get; init; }
    public Guid PathologistId { get; init; }
    public string Name { get; set; } = null!;
    public string? HeaderText { get; set; }
    public Guid? SignatureAssetId { get; set; }
}

internal sealed class PathologyReportRow
{
    public Guid Id { get; init; }
    public Guid PathologyCaseId { get; init; }
    public short Kind { get; set; }
    public DateTime ReceivedAtUtc { get; set; }
    public string OriginalStorageKey { get; set; } = null!;
    public string OriginalSha256 { get; set; } = null!;
    public string OriginalFileName { get; set; } = null!;
    public string OriginalMimeType { get; set; } = null!;
    public string ExtractedText { get; set; } = string.Empty;
    public string? ExternalReportNumber { get; set; }
    public Guid? PathologistId { get; set; }
    public Guid? TemplateId { get; set; }
    public Guid? RelatedAssayId { get; set; }
    public string? Notes { get; set; }
}

internal sealed class PathologyReportContainerRow
{
    public Guid PathologyReportId { get; init; }
    public Guid BiopsyContainerId { get; init; }
}

internal sealed class PathologyReportAssetRow
{
    public Guid PathologyReportId { get; init; }
    public Guid AssetId { get; init; }
    public short Role { get; set; }
    public string? OriginalName { get; set; }
}

internal sealed class PathologyReportAnnotationRow
{
    public Guid Id { get; init; }
    public Guid PathologyReportId { get; init; }
    public int Start { get; set; }
    public int Length { get; set; }
    public short Kind { get; set; }
    public string? CodeSystem { get; set; }
    public string? Code { get; set; }
    public Guid? AnatomicalSiteId { get; set; }
    public bool ConfirmedByUser { get; set; }
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
