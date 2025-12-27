using System.Collections.Concurrent;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Scheduling;

namespace GIPractice.Api.Scheduling;

public interface ISchedulingStore
{
    Task<CalendarDayMetaDto?> GetDayMetaAsync(DateOnly day, CancellationToken ct);
    Task UpsertDayMetaAsync(CalendarDayMetaDto meta, CancellationToken ct);

    Task<IReadOnlyList<AppointmentTypeDto>> GetAppointmentTypesAsync(CancellationToken ct);

    Task<IReadOnlyList<AppointmentListItemDto>> GetAppointmentsForDayAsync(DateOnly day, CancellationToken ct);

    Task<AppointmentListItemDto?> GetAppointmentAsync(AppointmentId id, CancellationToken ct);
    Task<AppointmentId> CreateAppointmentAsync(AppointmentListItemDto appt, CancellationToken ct);
    Task<bool> UpdateAppointmentAsync(AppointmentListItemDto appt, CancellationToken ct);
    Task<bool> DeleteAppointmentAsync(AppointmentId id, CancellationToken ct);
}

public sealed class InMemorySchedulingStore : ISchedulingStore
{
    private readonly ConcurrentDictionary<DateOnly, CalendarDayMetaDto> _meta = new();
    private readonly ConcurrentDictionary<AppointmentId, AppointmentListItemDto> _appointments = new();
    private int _nextAppointmentId = 1;

    private readonly List<AppointmentTypeDto> _types =
    [
        new AppointmentTypeDto(new(1), "Γαστροσκόπηση", 30, "#2D7DFF",
            ClinicCapability.DoctorOnSite | ClinicCapability.EndoscopySuiteOpen),

        new AppointmentTypeDto(new(2), "Κολονοσκόπηση", 60, "#FF7A2D",
            ClinicCapability.DoctorOnSite | ClinicCapability.EndoscopySuiteOpen),

        new AppointmentTypeDto(new(3), "Ορθοσιγμοειδοσκόπηση", 30, "#7A2DFF",
            ClinicCapability.DoctorOnSite | ClinicCapability.OrthoHeineOpen),

        new AppointmentTypeDto(new(4), "Κλινική εξέταση", 15, "#2DFF7A",
            ClinicCapability.DoctorOnSite),
    ];

    public Task<CalendarDayMetaDto?> GetDayMetaAsync(DateOnly day, CancellationToken ct)
        => Task.FromResult(_meta.TryGetValue(day, out var v) ? v : null);

    public Task UpsertDayMetaAsync(CalendarDayMetaDto meta, CancellationToken ct)
    {
        _meta[meta.Day] = meta;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<AppointmentTypeDto>> GetAppointmentTypesAsync(CancellationToken ct)
        => Task.FromResult<IReadOnlyList<AppointmentTypeDto>>(_types);

    public Task<IReadOnlyList<AppointmentListItemDto>> GetAppointmentsForDayAsync(DateOnly day, CancellationToken ct)
    {
        var start = day.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var end = start.AddDays(1);

        var list = _appointments.Values
            .Where(a => a.StartUtc >= start && a.StartUtc < end && a.Status != AppointmentStatus.Cancelled)
            .OrderBy(a => a.StartUtc)
            .ToList();

        return Task.FromResult<IReadOnlyList<AppointmentListItemDto>>(list);
    }

    public Task<AppointmentListItemDto?> GetAppointmentAsync(AppointmentId id, CancellationToken ct)
        => Task.FromResult(_appointments.TryGetValue(id, out var v) ? v : null);

    public Task<AppointmentId> CreateAppointmentAsync(AppointmentListItemDto appt, CancellationToken ct)
    {
        var id = new AppointmentId(_nextAppointmentId++);
        var created = appt with { Id = id };
        _appointments[id] = created;
        return Task.FromResult(id);
    }

    public Task<bool> UpdateAppointmentAsync(AppointmentListItemDto appt, CancellationToken ct)
    {
        if (!_appointments.ContainsKey(appt.Id))
            return Task.FromResult(false);

        _appointments[appt.Id] = appt;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAppointmentAsync(AppointmentId id, CancellationToken ct)
        => Task.FromResult(_appointments.TryRemove(id, out _));
}
