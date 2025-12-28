using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Infai;

public sealed record InfaiReportUpsertRequestDto(
    [param: NonZeroId] InfaiReportId? Id,

    [param: NonZeroId] PatientId PatientId,
    [param: NonZeroId] EndoscopyId EndoscopyId,

    [param: NotDefault] DateTime? SentUtc,
    [param: NotDefault] DateTime? ReceivedUtc,

    [param: MaxLength(120)] string? ParcelId,
    bool IsUrgent,

    [param: MaxLength(20000)] string? ReportText,
    [param: EnumDataType(typeof(InfaiReportStatus))] InfaiReportStatus Status,

    byte[]? RowVersion);
