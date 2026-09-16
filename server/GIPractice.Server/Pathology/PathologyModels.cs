namespace GIPractice.Server.Pathology;

public enum PathologyFeeWaiverReason
{
    None = 0,
    Doctor = 1,
    Other = 99
}

public enum PathologyChargeKind
{
    InitialBiopsy = 1,
    AdditionalAssay = 2,
    ManualAdjustment = 99
}

public enum PathologyAssayStatus
{
    Requested = 1,
    Performed = 2,
    Reported = 3,
    Cancelled = 4
}

public enum PathologyReportKind
{
    Initial = 1,
    Addendum = 2
}

public enum PathologyReportAnnotationKind
{
    TissueType = 1,
    Diagnosis = 2,
    Finding = 3,
    Organism = 4,
    Anatomy = 5,
    Other = 99
}

public enum PathologyDocumentAssetRole
{
    Signature = 1,
    HeaderImage = 2,
    EmbeddedImage = 3,
    Other = 99
}

public sealed record Pathologist(
    Guid Id,
    string DisplayName,
    string? Notes = null);

// One pathology case belongs to one Endoscopy and groups the practice-managed biopsy
// workflow, reports, later assays and billing that originated from that endoscopy.
public sealed record PathologyCase(
    Guid Id,
    Guid EndoscopyId,
    DateTimeOffset CreatedAtUtc,
    bool IsUrgent = false,
    bool ReceiptRequested = false,
    PathologyFeeWaiverReason FeeWaiverReason = PathologyFeeWaiverReason.None);

// The physical tracking unit. CollectionSiteText preserves exactly what the user typed
// for labels/display (for example "antrum-corpus"). Searchable meaning lives in one or
// more BiopsyContainerSite rows pointing to the controlled anatomy vocabulary.
public sealed record BiopsyContainer(
    Guid Id,
    Guid PathologyCaseId,
    string LabelCode,
    int Ordinal,
    string CollectionSiteText,
    DateTimeOffset CollectedAtUtc,
    string? Description = null,
    DateTimeOffset? ExternalReleasedAtUtc = null,
    string? ExternalReleaseNote = null)
{
    public bool IsExternallyReleased => ExternalReleasedAtUtc is not null;
}

public sealed record BiopsyContainerSite(
    Guid BiopsyContainerId,
    Guid AnatomicalSiteId);

// One courier shipment. The courier cost belongs to the shipment itself and is
// deliberately separate from pathology fees charged for biopsy work.
public sealed record Parcel(
    Guid Id,
    string ParcelNumber,
    Guid PathologistId,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? SentAtUtc = null,
    string? CourierName = null,
    string? TrackingNumber = null,
    decimal? CourierCost = null,
    string Currency = "EUR");

// Physical membership of practice-managed biopsy containers in a courier parcel.
// Initial biopsy pricing counts only containers from the same PathologyCase that are
// actually members of that Parcel.
public sealed record ParcelContainer(
    Guid ParcelId,
    Guid BiopsyContainerId);

public sealed record PathologyAssay(
    Guid Id,
    Guid PathologyCaseId,
    string AssayCode,
    DateTimeOffset RequestedAtUtc,
    PathologyAssayStatus Status = PathologyAssayStatus.Requested,
    Guid? BiopsyContainerId = null,
    string? Description = null);

// Financial ledger entry. Historical calculated and charged amounts are snapshots.
public sealed record PathologyCharge(
    Guid Id,
    Guid PathologyCaseId,
    PathologyChargeKind Kind,
    DateTimeOffset CreatedAtUtc,
    decimal CalculatedAmount,
    decimal ChargedAmount,
    string Currency,
    PathologyFeeWaiverReason WaiverReason = PathologyFeeWaiverReason.None,
    int? ContainerCountSnapshot = null,
    string? PricingPolicyCode = null,
    Guid? BiopsyContainerId = null,
    Guid? AssayId = null,
    Guid? BilledInParcelId = null,
    string? Description = null);

// Content-addressed asset extracted from source documents. Identical signatures/images
// can resolve to one asset by SHA-256 in the derived representation.
public sealed record PathologyDocumentAsset(
    Guid Id,
    string Sha256,
    string MimeType,
    string StorageKey,
    long ByteLength);

// Optional reusable presentation metadata for one pathologist. It is never used to
// reconstruct or replace the exact original report file.
public sealed record PathologyReportTemplate(
    Guid Id,
    Guid PathologistId,
    string Name,
    string? HeaderText = null,
    Guid? SignatureAssetId = null);

// Practice-managed pathology reports retain the exact source document and a parsed,
// searchable representation. OriginalStorageKey + OriginalSha256 preserve provenance;
// ExtractedText is what the application searches/annotates.
public sealed record PathologyReport(
    Guid Id,
    Guid PathologyCaseId,
    PathologyReportKind Kind,
    DateTimeOffset ReceivedAtUtc,
    string OriginalStorageKey,
    string OriginalSha256,
    string OriginalFileName,
    string OriginalMimeType,
    string ExtractedText,
    string? ExternalReportNumber = null,
    Guid? PathologistId = null,
    Guid? TemplateId = null,
    Guid? RelatedAssayId = null,
    string? Notes = null);

public sealed record PathologyReportContainer(
    Guid PathologyReportId,
    Guid BiopsyContainerId);

public sealed record PathologyReportAsset(
    Guid PathologyReportId,
    Guid AssetId,
    PathologyDocumentAssetRole Role,
    string? OriginalName = null);

// Parser-derived semantics over ExtractedText. As with clinical-exam annotations,
// suggestions are not treated as confirmed facts until accepted by a user.
public sealed record PathologyReportAnnotation(
    Guid Id,
    Guid PathologyReportId,
    int Start,
    int Length,
    PathologyReportAnnotationKind Kind,
    string? CodeSystem = null,
    string? Code = null,
    Guid? AnatomicalSiteId = null,
    bool ConfirmedByUser = false)
{
    public int End => Start + Length;
}
