using GIPractice.Core.Abstractions;

namespace GIPractice.Core.Entities;

public sealed class PathologyParcel : BaseEntity
{
    public int PathologistId { get; set; }
    public Pathologist? Pathologist { get; set; }

    public string ParcelCode { get; set; } = default!;

    public DateTime? DispatchedAtUtc { get; set; }
    public string? CourierName { get; set; }
    public string? TrackingNumber { get; set; }
    public string? Notes { get; set; }

    // Persisted total for the parcel/protocol. Computed from the included endoscopies.
    public decimal MonetarySum { get; set; }

    public byte[] RowVersion { get; set; } = [];

    public List<PathologyReport> Reports { get; set; } = [];
}
