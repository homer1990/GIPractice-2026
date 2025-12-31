using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public sealed record PathologyReportListItemDto(
    PathologyReportId Id,

    PatientId PatientId,
    EndoscopyId EndoscopyId,
    PathologistId PathologistId,

    // derived / display id, not DB id
    string PathologistRecordId,

    string? DispatchParcelCode,

    PathologyReportStatus Status,
    bool IsUrgent,

    DateTime? SentAtUtc,
    DateTime? ReceivedAtUtc,

    PathologyDocumentKind? DocumentKind,

    byte[]? RowVersion);