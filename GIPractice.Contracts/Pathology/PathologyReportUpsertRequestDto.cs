using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathology;

public sealed record PathologyReportUpsertRequestDto(
    [property: NonZeroId] PathologyReportId? Id,

    [property: NonZeroId] PatientId PatientId,
    [property: NonZeroId] EndoscopyId EndoscopyId,
    [property: NonZeroId] BiopsyDispatchBundleId? BiopsyDispatchBundleId,

    [property: NotDefault] DateTime? SentUtc,
    [property: NotDefault] DateTime? ReceivedUtc,

    [property: MaxLength(120)] string? ParcelId,
    bool IsUrgent,

    [property: MaxLength(20000)] string? ReportText,
    [property: EnumDataType(typeof(PathologyReportStatus))] PathologyReportStatus Status,

    byte[]? RowVersion);
