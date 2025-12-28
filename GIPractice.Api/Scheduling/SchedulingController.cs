using GIPractice.Contracts.Scheduling;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GIPractice.Api.Common;

namespace GIPractice.Api.Scheduling;

[ApiController]
[Route("api/scheduling")]
public sealed class SchedulingController : ControllerBase
{
    private readonly ISchedulingService _svc;

    public SchedulingController(ISchedulingService svc) => _svc = svc;

    [HttpGet("day/{day}")]
    public Task<IActionResult> GetDay([FromRoute] DateOnly day, CancellationToken ct)
        => _svc.GetScheduleDayAsync(new ScheduleDayRequestDto(day), ct).ToActionResultAsync(HttpContext);

    [HttpGet("day/{day}/available")]
    public Task<IActionResult> GetAvailable(
        [FromRoute] DateOnly day,
        [FromQuery] int appointmentTypeId,
        [FromQuery] int? slotStepMinutes,
        CancellationToken ct)
        => _svc.GetAvailableStartTimesAsync(
            new GetAvailableStartTimesRequestDto(day, new(appointmentTypeId), slotStepMinutes ?? 30), ct).ToActionResultAsync(HttpContext);

    [HttpPost("appointments")]
    public Task<IActionResult> Create([FromBody] AppointmentUpsertRequestDto dto, CancellationToken ct)
        => _svc.CreateAppointmentAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpPut("appointments")]
    public Task<IActionResult> Update([FromBody] AppointmentUpsertRequestDto dto, CancellationToken ct)
        => _svc.UpdateAppointmentAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpDelete("appointments/{id:int}")]
    public Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        => _svc.DeleteAppointmentAsync(new(id), ct).ToActionResultAsync(HttpContext);

    [HttpPost("appointments/resolve")]
    public Task<IActionResult> Resolve([FromBody] AppointmentResolveRequestDto dto, CancellationToken ct)
        => _svc.ResolveAppointmentAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpPut("day-meta")]
    public Task<IActionResult> UpsertDayMeta([FromBody] CalendarDayMetaUpsertDto dto, CancellationToken ct)
        => _svc.UpsertCalendarDayMetaAsync(dto, ct).ToActionResultAsync(HttpContext);
}
