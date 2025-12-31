using GIPractice.Core.Abstractions;

namespace GIPractice.Core.Entities;

public sealed class PathologyParcel : BaseEntity
{
    public int PathologistId { get; set; }
    public Pathologist Pathologist { get; set; } = null!;

    // Your “2018 style” identifier (unique per pathologist, sortable)
    public string ParcelCode { get; set; } = string.Empty;
    public DateTime? DispatchedAtUtc { get; set; }

    public string? CourierName { get; set; }
    public string? TrackingNumber { get; set; }
    public string? Notes { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public List<PathologyReport> Reports { get; set; } = [];
}
