using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Infai;

public sealed record InfaiReportUpsertRequestDto(
    [property: NonZeroId] InfaiReportId? Id,

    [property: NonZeroId] PatientId PatientId,
    [property: NonZeroId] EndoscopyId EndoscopyId,

    [property: NotDefault] DateTime? SentUtc,
    [property: NotDefault] DateTime? ReceivedUtc,

    [property: MaxLength(120)] string? ParcelId,
    bool IsUrgent,

    [property: MaxLength(20000)] string? ReportText,
    [property: EnumDataType(typeof(InfaiReportStatus))] InfaiReportStatus Status,

    byte[]? RowVersion);
