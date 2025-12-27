using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Scheduling;

public interface ISchedulingService
{
    Task<ResultDto<ScheduleDayDto>> GetScheduleDayAsync(
        ScheduleDayRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<GetAvailableStartTimesResponseDto>> GetAvailableStartTimesAsync(
        GetAvailableStartTimesRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<AppointmentId>> CreateAppointmentAsync(
        AppointmentUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> UpdateAppointmentAsync(
        AppointmentUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> DeleteAppointmentAsync(
        AppointmentId id,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> UpsertCalendarDayMetaAsync(
        CalendarDayMetaUpsertDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<AppointmentResolveResponseDto>> ResolveAppointmentAsync(
        AppointmentResolveRequestDto request,
        CancellationToken cancellationToken = default);
}
