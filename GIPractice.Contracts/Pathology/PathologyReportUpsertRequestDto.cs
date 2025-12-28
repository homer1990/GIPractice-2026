using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public sealed record PathologyReportUpsertRequestDto(
    [param: NonZeroId] PathologyReportId? Id,

    [param: NonZeroId] PatientId PatientId,
    [param: NonZeroId] EndoscopyId EndoscopyId,
    [param: NonZeroId] BiopsyDispatchBundleId? BiopsyDispatchBundleId,

    [param: NotDefault] DateTime? SentUtc,
    [param: NotDefault] DateTime? ReceivedUtc,

    [param: MaxLength(120)] string? ParcelId,
    bool IsUrgent,

    [param: MaxLength(20000)] string? ReportText,
    [param: EnumDataType(typeof(PathologyReportStatus))] PathologyReportStatus Status,

    byte[]? RowVersion);
