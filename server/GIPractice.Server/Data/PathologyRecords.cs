namespace GIPractice.Server.Data;

internal sealed class PathologistRow
{
    public Guid Id { get; init; }
    public string DisplayName { get; set; } = null!;
    public string? Notes { get; set; }
}

internal sealed class PathologyCaseRow
{
    public Guid Id { get; init; }
    public Guid EndoscopyId { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public bool IsUrgent { get; set; }
    public bool ReceiptRequested { get; set; }
    public short FeeWaiverReason { get; set; }
    public Guid? AssignedPathologistId { get; set; }
}

internal sealed class BiopsyContainerRow
{
    public Guid Id { get; init; }
    public Guid PathologyCaseId { get; init; }
    public string LabelCode { get; set; } = null!;
    public int Ordinal { get; set; }
    public string AnatomicalSiteCode { get; set; } = null!;
    public DateTime CollectedAtUtc { get; set; }
    public string? Description { get; set; }
}

internal sealed class ParcelRow
{
    public Guid Id { get; init; }
    public string ParcelNumber { get; set; } = null!;
    public Guid PathologistId { get; init; }
    public DateTime CreatedAtUtc { get; init; }
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
    public Guid? BiopsyContainerId { get; set; }
    public string AssayCode { get; set; } = null!;
    public DateTime RequestedAtUtc { get; set; }
    public short Status { get; set; }
    public string? Description { get; set; }
}

internal sealed class PathologyChargeRow
{
    public Guid Id { get; init; }
    public Guid PathologyCaseId { get; init; }
    public short Kind { get; set; }
    public DateTime CreatedAtUtc { get; init; }
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

internal sealed class PathologyReportRow
{
    public Guid Id { get; init; }
    public Guid PathologyCaseId { get; init; }
    public short Kind { get; set; }
    public DateTime ReceivedAtUtc { get; set; }
    public string StorageKey { get; set; } = null!;
    public string? ExternalReportNumber { get; set; }
    public Guid? RelatedAssayId { get; set; }
    public string? Notes { get; set; }
}
