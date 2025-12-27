using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public sealed record SchedulerNotificationSearchRequestDto(
    DateOnly? Day = null,                 // if you want “today’s queue”
    bool IncludeClosed = false,
    SchedulerNotificationKind? Kind = null,
    PatientId? PatientId = null,
    PagedRequestDto? Paging = null);
