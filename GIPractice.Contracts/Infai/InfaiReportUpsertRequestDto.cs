using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Infai;

public sealed record InfaiReportUpsertRequestDto(
    InfaiReportId? Id,
    PatientId PatientId,
    EndoscopyId EndoscopyId,

    DateTime? SentUtc,
    DateTime? ReceivedUtc,

    string? ParcelId,
    bool IsUrgent,

    string? ReportText,
    InfaiReportStatus Status,

    byte[]? RowVersion);
