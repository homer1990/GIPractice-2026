using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public sealed record PathologyReportListItemDto(
    PathologyReportId Id,
    PatientId PatientId,
    EndoscopyId EndoscopyId,
    BiopsyDispatchBundleId? BiopsyDispatchBundleId,
    DateTime? SentUtc,
    DateTime? ReceivedUtc,
    bool IsUrgent,
    PathologyReportStatus Status);
