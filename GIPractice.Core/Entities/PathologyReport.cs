using GIPractice.Core.Abstractions;
using GIPractice.Core.Enums;

namespace GIPractice.Core.Entities;

public sealed class PathologyReport : BaseEntity
{
    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public int EndoscopyId { get; set; }
    public Endoscopy Endoscopy { get; set; } = null!;

    public int PathologistId { get; set; }
    public Pathologist Pathologist { get; set; } = null!;

    public int? PathologyParcelId { get; set; }
    public PathologyParcel? Parcel { get; set; }

    // Report lifecycle timestamps (filterable)
    public DateTime? SentAtUtc { get; set; }
    public DateTime? ReceivedAtUtc { get; set; }

    // Report content
    public string? ClinicalInfo { get; set; }
    public string? MacroscopyText { get; set; }
    public string? DiagnosisText { get; set; }
    public string? Notes { get; set; }

    public PathologyReportStatus Status { get; set; } = PathologyReportStatus.Draft;
    public bool IsUrgent { get; set; }

    // Report file
    public int? DocumentFileId { get; set; }
    public MediaFile? DocumentFile { get; set; }

    public PathologyDocumentKind DocumentKind { get; set; } = PathologyDocumentKind.Unknown;

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
