using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Infai;

public sealed record InfaiReportDto(
    InfaiReportId Id,
    PatientId PatientId,
    EncounterId EncounterId,

    DateTime? SentUtc,
    DateTime? ReceivedUtc,

    string? ExternalReference,
    string? ReportText,
    InfaiReportStatus Status,

    string? TrackingNumber,
    byte[]? RowVersion);
