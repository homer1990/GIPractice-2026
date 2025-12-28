using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Infai;

public sealed record InfaiReportListItemDto(
    InfaiReportId Id,
    PatientId PatientId,
    EndoscopyId EndoscopyId,
    DateTime? SentUtc,
    DateTime? ReceivedUtc,
    bool IsUrgent,
    InfaiReportStatus Status);
