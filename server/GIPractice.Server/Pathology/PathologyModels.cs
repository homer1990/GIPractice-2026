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

// One pathology case belongs to one Endoscopy and groups all biopsy containers,
// reports, later assays and billing belonging to that endoscopy.
public sealed record PathologyCase(
    Guid Id,
    Guid EndoscopyId,
    DateTimeOffset CreatedAtUtc,
    bool IsUrgent = false,
    bool ReceiptRequested = false,
    PathologyFeeWaiverReason FeeWaiverReason = PathologyFeeWaiverReason.None,
    Guid? AssignedPathologistId = null);

// The physical tracking unit. LabelCode is human-readable and globally unique,
// while Id remains the database identity.
public sealed record BiopsyContainer(
    Guid Id,
    Guid PathologyCaseId,
    string LabelCode,
    int Ordinal,
    string AnatomicalSiteCode,
    DateTimeOffset CollectedAtUtc,
    string? Description = null);

// One courier shipment. The courier cost belongs to the shipment itself and is
// deliberately separate from pathology fees charged per Endoscopy/case.
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

// Physical membership of biopsy containers in a parcel.
public sealed record ParcelContainer(
    Guid ParcelId,
    Guid BiopsyContainerId);

// A later assay may refer to one specific container or to the pathology case as a whole.
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

// Reports belong to the pathology case, not to a parcel. Additional assays can produce
// later addenda while still referring back to the original Endoscopy/containers.
public sealed record PathologyReport(
    Guid Id,
    Guid PathologyCaseId,
    PathologyReportKind Kind,
    DateTimeOffset ReceivedAtUtc,
    string StorageKey,
    string? ExternalReportNumber = null,
    Guid? RelatedAssayId = null,
    string? Notes = null);
