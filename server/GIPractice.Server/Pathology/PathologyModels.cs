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

public sealed record Pathologist(
    Guid Id,
    string DisplayName,
    string? Notes = null);

// One pathology case belongs to one Endoscopy and groups the practice-managed biopsy
// workflow, reports, later assays and billing that originated from that endoscopy.
// Routing is intentionally NOT stored here: individual containers may be released
// externally instead of entering the practice's pathology workflow.
public sealed record PathologyCase(
    Guid Id,
    Guid EndoscopyId,
    DateTimeOffset CreatedAtUtc,
    bool IsUrgent = false,
    bool ReceiptRequested = false,
    PathologyFeeWaiverReason FeeWaiverReason = PathologyFeeWaiverReason.None);

// The physical tracking unit. LabelCode is human-readable and globally unique,
// while Id remains the database identity. A container released directly to the patient
// or another external destination remains documented here, but it does not enter Parcel,
// Pathologist, PathologyReport or practice billing workflow.
public sealed record BiopsyContainer(
    Guid Id,
    Guid PathologyCaseId,
    string LabelCode,
    int Ordinal,
    string AnatomicalSiteCode,
    DateTimeOffset CollectedAtUtc,
    string? Description = null,
    DateTimeOffset? ExternalReleasedAtUtc = null,
    string? ExternalReleaseNote = null)
{
    public bool IsExternallyReleased => ExternalReleasedAtUtc is not null;
}

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
// The initial biopsy fee is calculated from the number of containers from one
// PathologyCase that are members of the parcel, not from every container collected
// during that Endoscopy.
public sealed record ParcelContainer(
    Guid ParcelId,
    Guid BiopsyContainerId);

// A later assay may refer to one specific practice-managed container or to the
// pathology case as a whole.
public sealed record PathologyAssay(
    Guid Id,
    Guid PathologyCaseId,
    string AssayCode,
    DateTimeOffset RequestedAtUtc,
    PathologyAssayStatus Status = PathologyAssayStatus.Requested,
    Guid? BiopsyContainerId = null,
    string? Description = null);

// Financial ledger entry. A charge can be created now and billed in a later parcel.
// CalculatedAmount preserves what the pricing rules produced; ChargedAmount preserves
// what was actually charged after waivers/adjustments.
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

// Reports here are reports received through the practice-managed pathology workflow.
// Outside reports from containers released externally are recorded in the patient's
// longitudinal history instead of creating a parallel external-pathology subsystem.
// Report coverage is explicit because a case can contain more containers than were
// actually submitted to this pathologist.
public sealed record PathologyReport(
    Guid Id,
    Guid PathologyCaseId,
    PathologyReportKind Kind,
    DateTimeOffset ReceivedAtUtc,
    string StorageKey,
    string? ExternalReportNumber = null,
    Guid? PathologistId = null,
    Guid? RelatedAssayId = null,
    string? Notes = null);

public sealed record PathologyReportContainer(
    Guid PathologyReportId,
    Guid BiopsyContainerId);
