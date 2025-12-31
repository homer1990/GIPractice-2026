using GIPractice.Core.Abstractions;

namespace GIPractice.Core.Entities;

public sealed class Pathologist : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    // Keep JSON for now (later: owned type + JSON column).
    public string PricingPlanJson { get; set; } = "{}";

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public List<PathologyParcel> Parcels { get; set; } = [];
    public List<PathologyReport> Reports { get; set; } = [];
}
