using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Scheduling;
using Microsoft.Extensions.Options;

namespace GIPractice.Api.Scheduling;

public sealed class SchedulingService(ISchedulingStore store, IOptions<SchedulerOptions> options) : ISchedulingService
{
    private readonly ISchedulingStore _store = store;
    private readonly SchedulerOptions _opt = options.Value;

    public async Task<ResultDto<ScheduleDayDto>> GetScheduleDayAsync(
        ScheduleDayRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var meta = await _store.GetDayMetaAsync(request.Date, cancellationToken)
                   ?? new CalendarDayMetaDto(
                       Day: request.Date,
                       IsHoliday: false,
                       IsDayOff: false,
                       Notes: null,
                       Capabilities: _opt.DefaultCapabilities,
                       RowVersion: null);

        var types = await _store.GetAppointmentTypesAsync(cancellationToken);
        var appts = await _store.GetAppointmentsForDayAsync(request.Date, cancellationToken);

        // TODO: wire these up once the corresponding stores/entities exist
        var openReschedule = Array.Empty<OpenRescheduleItemDto>();
        var notifications = Array.Empty<SchedulerNotificationDto>();

        return ResultDto<ScheduleDayDto>.Ok(new ScheduleDayDto(meta, types, appts, openReschedule, notifications));
    }

    public async Task<ResultDto<GetAvailableStartTimesResponseDto>> GetAvailableStartTimesAsync(
        GetAvailableStartTimesRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var meta = await _store.GetDayMetaAsync(request.Day, cancellationToken)
                   ?? new CalendarDayMetaDto(
                       Day: request.Day,
                       IsHoliday: false,
                       IsDayOff: false,
                       Notes: null,
                       Capabilities: _opt.DefaultCapabilities,
                       RowVersion: null);

        var types = await _store.GetAppointmentTypesAsync(cancellationToken);
        var type = types.FirstOrDefault(t => t.Id == request.AppointmentTypeId);
        if (type is null)
            return ResultDto<GetAvailableStartTimesResponseDto>.Fail("not_found", "Unknown appointment type.");

        var duration = TimeSpan.FromMinutes(type.MeanDurationMinutes);
        var step = TimeSpan.FromMinutes(Math.Max(5, request.SlotStepMinutes));

        var dayStart = DateTime.SpecifyKind(request.Day.ToDateTime(_opt.WorkdayStart), DateTimeKind.Utc);
        var dayEnd = DateTime.SpecifyKind(request.Day.ToDateTime(_opt.WorkdayEnd), DateTimeKind.Utc);

        // Capability gate (day can be “partially open”)
        var requiredCaps = type.RequiredCapabilities;
        var dayCaps = meta.Capabilities;
        var capsOk = (dayCaps & requiredCaps) == requiredCaps;
        var capsReason = capsOk ? null : BuildCapabilityBlockReason(dayCaps, requiredCaps);

        // If capabilities are missing, we still return the slot grid but mark all as unavailable
        var appts = await _store.GetAppointmentsForDayAsync(request.Day, cancellationToken);
        var busy = appts
            .Where(a => a.Status != AppointmentStatus.Cancelled)
            .Select(a => (Start: a.StartUtc, End: a.StartUtc.AddMinutes(a.DurationMinutes)))
            .OrderBy(x => x.Start)
            .ToList();

        var slots = new List<TimeSlotDto>();

        for (var t = dayStart; t + duration <= dayEnd; t = t.Add(step))
        {
            var candidateStart = t;
            var candidateEnd = t + duration;

            if (!capsOk)
            {
                slots.Add(new TimeSlotDto(
                    StartUtc: candidateStart,
                    EndUtc: candidateEnd,
                    IsAvailable: false,
                    BlockReason: capsReason));
                continue;
            }

            var overlap = busy.Any(b => IntervalsOverlap(candidateStart, candidateEnd, b.Start, b.End));

            slots.Add(new TimeSlotDto(
                StartUtc: candidateStart,
                EndUtc: candidateEnd,
                IsAvailable: !overlap,
                BlockReason: overlap ? "Overlaps existing appointment" : null));
        }

        return ResultDto<GetAvailableStartTimesResponseDto>.Ok(new GetAvailableStartTimesResponseDto(slots));
    }

    public async Task<ResultDto<AppointmentId>> CreateAppointmentAsync(
        AppointmentUpsertRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var ok = await IsSlotAvailableAsync(
            day: DateOnly.FromDateTime(request.StartUtc),
            startUtc: request.StartUtc,
            duration: TimeSpan.FromMinutes(request.DurationMinutes),
            appointmentTypeId: request.AppointmentTypeId,
            excludeAppointmentId: null,
            ct: cancellationToken);

        if (!ok)
            return ResultDto<AppointmentId>.Fail("conflict", "Selected time is not available.");

        // For now keep denormalized fields dummy; later hydrate from Patient/Types tables
        var appt = new AppointmentListItemDto(
            Id: new AppointmentId(0),
            PatientId: request.PatientId,
            PatientFullName: "(dummy)",
            PatientPhoneNumber: null,
            StartUtc: request.StartUtc,
            DurationMinutes: request.DurationMinutes,
            AppointmentTypeId: request.AppointmentTypeId,
            AppointmentTypeName: request.AppointmentTypeName,
            Status: request.Status,
            IsUrgent: request.IsUrgent,
            Notes: request.Notes,
            EncounterId: null,
            RowVersion: request.RowVersion);

        var id = await _store.CreateAppointmentAsync(appt, cancellationToken);
        return ResultDto<AppointmentId>.Ok(id);
    }

    public async Task<ResultDto<bool>> UpdateAppointmentAsync(
        AppointmentUpsertRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (request.Id is null)
            return ResultDto<bool>.Fail("invalid", "Missing appointment id.");

        var id = request.Id.Value;
        var existing = await _store.GetAppointmentAsync(id, cancellationToken);
        if (existing is null)
            return ResultDto<bool>.Fail("not_found", "Appointment not found.");

        var ok = await IsSlotAvailableAsync(
            day: DateOnly.FromDateTime(request.StartUtc),
            startUtc: request.StartUtc,
            duration: TimeSpan.FromMinutes(request.DurationMinutes),
            appointmentTypeId: request.AppointmentTypeId,
            excludeAppointmentId: id,
            ct: cancellationToken);

        if (!ok)
            return ResultDto<bool>.Fail("conflict", "Selected time is not available.");

        var updated = existing with
        {
            StartUtc = request.StartUtc,
            DurationMinutes = request.DurationMinutes,
            AppointmentTypeId = request.AppointmentTypeId,
            AppointmentTypeName = request.AppointmentTypeName,
            Status = request.Status,
            IsUrgent = request.IsUrgent,
            Notes = request.Notes,
            RowVersion = request.RowVersion
        };

        var saved = await _store.UpdateAppointmentAsync(updated, cancellationToken);
        return ResultDto<bool>.Ok(saved);
    }

    public async Task<ResultDto<bool>> DeleteAppointmentAsync(AppointmentId id, CancellationToken cancellationToken = default)
    {
        var ok = await _store.DeleteAppointmentAsync(id, cancellationToken);
        return ResultDto<bool>.Ok(ok);
    }

    public async Task<ResultDto<bool>> UpsertCalendarDayMetaAsync(
        CalendarDayMetaUpsertDto request,
        CancellationToken cancellationToken = default)
    {
        var existing = await _store.GetDayMetaAsync(request.Day, cancellationToken)
                       ?? new CalendarDayMetaDto(
                           Day: request.Day,
                           IsHoliday: false,
                           IsDayOff: false,
                           Notes: null,
                           Capabilities: _opt.DefaultCapabilities,
                           RowVersion: null);

        var updated = existing with
        {
            IsHoliday = request.IsHoliday,
            IsDayOff = request.IsDayOff,
            Notes = request.Notes
        };

        await _store.UpsertDayMetaAsync(updated, cancellationToken);
        return ResultDto<bool>.Ok(true);
    }

    public Task<ResultDto<AppointmentResolveResponseDto>> ResolveAppointmentAsync(
        AppointmentResolveRequestDto request,
        CancellationToken cancellationToken = default)
    {
        // TODO later: create Encounter record and link it
        return Task.FromResult(ResultDto<AppointmentResolveResponseDto>.Ok(new AppointmentResolveResponseDto(new EncounterId(1))));
    }

    private async Task<bool> IsSlotAvailableAsync(
        DateOnly day,
        DateTime startUtc,
        TimeSpan duration,
        AppointmentTypeId appointmentTypeId,
        AppointmentId? excludeAppointmentId,
        CancellationToken ct)
    {
        var meta = await _store.GetDayMetaAsync(day, ct)
                   ?? new CalendarDayMetaDto(
                       Day: day,
                       IsHoliday: false,
                       IsDayOff: false,
                       Notes: null,
                       Capabilities: _opt.DefaultCapabilities,
                       RowVersion: null);

        var types = await _store.GetAppointmentTypesAsync(ct);
        var type = types.FirstOrDefault(t => t.Id == appointmentTypeId);
        if (type is null)
            return false;

        // Capability gate
        if ((meta.Capabilities & type.RequiredCapabilities) != type.RequiredCapabilities)
            return false;

        var dayStart = DateTime.SpecifyKind(day.ToDateTime(_opt.WorkdayStart), DateTimeKind.Utc);
        var dayEnd = DateTime.SpecifyKind(day.ToDateTime(_opt.WorkdayEnd), DateTimeKind.Utc);

        var endUtc = startUtc + duration;
        if (startUtc < dayStart || endUtc > dayEnd)
            return false;

        var appts = await _store.GetAppointmentsForDayAsync(day, ct);
        var busy = appts
            .Where(a => a.Status != AppointmentStatus.Cancelled)
            .Where(a => excludeAppointmentId is null || a.Id != excludeAppointmentId.Value)
            .Select(a => (Start: a.StartUtc, End: a.StartUtc.AddMinutes(a.DurationMinutes)));

        return !busy.Any(b => IntervalsOverlap(startUtc, endUtc, b.Start, b.End));
    }

    private static bool IntervalsOverlap(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd)
        => aStart < bEnd && bStart < aEnd; // [start,end)

    private static string BuildCapabilityBlockReason(ClinicCapability dayCaps, ClinicCapability requiredCaps)
    {
        var missing = requiredCaps & ~dayCaps;
        if (missing == ClinicCapability.None)
            return "Capabilities mismatch";

        var parts = new List<string>();

        if (missing.HasFlag(ClinicCapability.FrontDeskOpen)) parts.Add("Front desk closed");
        if (missing.HasFlag(ClinicCapability.DoctorOnSite)) parts.Add("Doctor not on site");
        if (missing.HasFlag(ClinicCapability.EndoscopySuiteOpen)) parts.Add("Endoscopy suite closed");
        if (missing.HasFlag(ClinicCapability.OrthoHeineOpen)) parts.Add("Ortho/HEINE closed");
        if (missing.HasFlag(ClinicCapability.BiopsyHandling)) parts.Add("Biopsy handling unavailable");
        if (missing.HasFlag(ClinicCapability.Reporting)) parts.Add("Reporting unavailable");

        return string.Join(", ", parts);
    }
}
