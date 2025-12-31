namespace GIPractice.Contracts.Pathology;

public sealed record PathologyReportDocumentDto(
    [param: EnumDataType(typeof(PathologyDocumentKind))] PathologyDocumentKind Kind,
    [param: Required, MaxLength(200)] string FileName,
    [param: Required, MaxLength(100)] string ContentType,
    [param: Required] byte[] Bytes);
