using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public sealed record PathologyReportDto(
    PathologyReportId Id,
    PatientId PatientId,
    EndoscopyId EndoscopyId,
    DispatchBundleId? DispatchBundleId,

    DateTime? SentUtc,
    DateTime? ReceivedUtc,

    string? ParcelId,
    bool IsUrgent,

    string? ReportText,
    PathologyReportStatus Status,

    byte[]? RowVersion);
