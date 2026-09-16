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

public enum BiopsyTransferKind
{
    CourierParcel = 1,
    HandedToPatient = 2,
    HandedToThirdParty = 3,
    Other = 99
}

public sealed record Pathologist(
    Guid Id,
    string DisplayName,
    string? Notes = null);

// One pathology case belongs to one Endoscopy and groups the biopsy containers,
// reports, later assays and billing that originated from that endoscopy.
// Routing is intentionally NOT stored here: containers from one case may go to
// different destinations.
public sealed record PathologyCase(
    Guid Id,
    Guid EndoscopyId,
    DateTimeOffset CreatedAtUtc,
    bool IsUrgent = false,
    bool ReceiptRequested = false,
    PathologyFeeWaiverReason FeeWaiverReason = PathologyFeeWaiverReason.None);

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

// Physical membership of biopsy containers in a courier parcel.
public sealed record ParcelContainer(
    Guid ParcelId,
    Guid BiopsyContainerId);

// A transfer records physical custody when a container leaves the practice by a route
// other than (or in addition to modelling) a normal courier parcel. This supports cases
// such as handing an urgent container to the patient for their oncologist while other
// containers from the same endoscopy remain for the practice's normal pathology flow.
public sealed record BiopsyTransfer(
    Guid Id,
    Guid BiopsyContainerId,
    BiopsyTransferKind Kind,
    DateTimeOffset TransferredAtUtc,
    Guid? ParcelId = null,
    Guid? PathologistId = null,
    string? RecipientName = null,
    string? Notes = null);

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

// Reports belong to the originating pathology case, not to a parcel. Because containers
// from one endoscopy may be split between destinations, report coverage is explicit via
// PathologyReportContainer rather than assuming that a report covers the whole case.
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
