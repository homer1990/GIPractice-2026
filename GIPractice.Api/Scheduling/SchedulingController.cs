using GIPractice.Contracts.Scheduling;
using Microsoft.AspNetCore.Mvc;

namespace GIPractice.Api.Scheduling;

[ApiController]
[Route("api/scheduling")]
public sealed class SchedulingController : ControllerBase
{
    private readonly ISchedulingService _svc;

    public SchedulingController(ISchedulingService svc) => _svc = svc;

    [HttpGet("day/{day}")]
    public Task<IActionResult> GetDay([FromRoute] DateOnly day, CancellationToken ct)
        => Wrap(_svc.GetScheduleDayAsync(new ScheduleDayRequestDto(day), ct));

    [HttpGet("day/{day}/available")]
    public Task<IActionResult> GetAvailable(
        [FromRoute] DateOnly day,
        [FromQuery] int appointmentTypeId,
        [FromQuery] int? slotStepMinutes,
        CancellationToken ct)
        => Wrap(_svc.GetAvailableStartTimesAsync(
            new GetAvailableStartTimesRequestDto(day, new(appointmentTypeId), slotStepMinutes ?? 30), ct));

    [HttpPost("appointments")]
    public Task<IActionResult> Create([FromBody] AppointmentUpsertRequestDto dto, CancellationToken ct)
        => Wrap(_svc.CreateAppointmentAsync(dto, ct));

    [HttpPut("appointments")]
    public Task<IActionResult> Update([FromBody] AppointmentUpsertRequestDto dto, CancellationToken ct)
        => Wrap(_svc.UpdateAppointmentAsync(dto, ct));

    [HttpDelete("appointments/{id:int}")]
    public Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        => Wrap(_svc.DeleteAppointmentAsync(id, ct));

    [HttpPost("appointments/resolve")]
    public Task<IActionResult> Resolve([FromBody] AppointmentResolveRequestDto dto, CancellationToken ct)
        => Wrap(_svc.ResolveAppointmentAsync(dto, ct));

    [HttpPut("day-meta")]
    public Task<IActionResult> UpsertDayMeta([FromBody] CalendarDayMetaDto dto, CancellationToken ct)
        => Wrap(_svc.UpsertCalendarDayMetaAsync(dto, ct));

    private static async Task<IActionResult> Wrap<T>(Task<GIPractice.Contracts.Common.ResultDto<T>> task)
    {
        var res = await task;
        return res.IsSuccess ? new OkObjectResult(res) : new BadRequestObjectResult(res);
    }
}
