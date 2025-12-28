using System.ComponentModel.DataAnnotations;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Common.Validation;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public sealed record SchedulerNotificationSearchRequestDto(
    [param: NotDefault] DateOnly? Day = null,
    bool IncludeClosed = false,
    SchedulerNotificationKind? Kind = null,
    [param: NonZeroId] PatientId? PatientId = null,
    PagedRequestDto? Paging = null);
