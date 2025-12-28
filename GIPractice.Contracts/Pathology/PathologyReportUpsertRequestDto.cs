using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public sealed record PathologyReportUpsertRequestDto(
    PathologyReportId? Id,
    PatientId PatientId,
    EndoscopyId EndoscopyId,
    BiopsyDispatchBundleId? BiopsyDispatchBundleId,

    DateTime? SentUtc,
    DateTime? ReceivedUtc,

    string? ParcelId,
    bool IsUrgent,

    string? ReportText,
    PathologyReportStatus Status,

    byte[]? RowVersion);
