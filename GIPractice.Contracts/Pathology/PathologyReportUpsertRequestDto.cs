using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public sealed record PathologyReportUpsertRequestDto(
    [param: NonZeroId] PathologyReportId? Id,

    [param: NonZeroId] PatientId PatientId,
    [param: NonZeroId] EndoscopyId EndoscopyId,
    [param: NonZeroId] PathologistId PathologistId,
    [param: NonZeroId] PathologyParcelId? DispatchParcelId, 

    [param: NotDefault] DateTime? SentAtUtc,
    [param: NotDefault] DateTime? ReceivedAtUtc,

    [param: MaxLength(2000)] string? Notes,
    [param: MaxLength(2000)] string? ClinicalInfo,
    [param: MaxLength(8000)] string? MacroscopyText,
    [param: MaxLength(8000)] string? DiagnosisText,

    [param: EnumDataType(typeof(PathologyReportStatus))] PathologyReportStatus Status,
    bool IsUrgent,

    // If you store docs as MediaFiles, set this.
    [param: NonZeroId] MediaFileId? DocumentFileId,

    [param: EnumDataType(typeof(PathologyDocumentKind))] PathologyDocumentKind DocumentKind,

    [param: MaxLength(200)] string? DocumentFileName,
    [param: MaxLength(100)] string? DocumentContentType,

    // Optional inline doc (dev/tests). In prod you can keep this null.
    PathologyReportDocumentDto? Document,

    byte[]? RowVersion);