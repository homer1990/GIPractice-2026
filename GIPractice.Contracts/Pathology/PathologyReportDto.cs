using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public sealed record PathologyReportDto(
    PathologyReportId Id,

    PatientId PatientId,
    EndoscopyId EndoscopyId,
    PathologistId PathologistId,

    string PathologistRecordId,

    string? DispatchParcelCode,

    DateTime? SentAtUtc,
    DateTime? ReceivedAtUtc,

    string? Notes,
    string? ClinicalInfo,
    string? MacroscopyText,
    string? DiagnosisText,

    PathologyReportStatus Status,
    bool IsUrgent,

    MediaFileId? DocumentFileId,
    PathologyDocumentKind DocumentKind,
    string? DocumentFileName,
    string? DocumentContentType,

    PathologyReportDocumentDto? Document,

    byte[]? RowVersion);