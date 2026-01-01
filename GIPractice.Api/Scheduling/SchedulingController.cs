using GIPractice.Api.Common;
using GIPractice.Api.Auth;
using GIPractice.Contracts.Auth;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Scheduling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GIPractice.Api.Scheduling;

[ApiController]
[Route("api/scheduling")]
[Authorize(Roles = RoleSets.Staff)]
public sealed class SchedulingController(ISchedulingService svc) : ControllerBase
{
    private readonly ISchedulingService _svc = svc;

    // GET /api/scheduling/day/2025-12-27
    [HttpGet("day/{day}")]
    public Task<IActionResult> GetDay([FromRoute] DateOnly day, CancellationToken ct)
        => _svc.GetScheduleDayAsync(new ScheduleDayRequestDto(day), ct)
            .ToActionResultAsync(HttpContext);

    // GET /api/scheduling/day/2025-12-27/available/2?slotStepMinutes=15
    [HttpGet("day/{day}/available/{appointmentTypeId:int}")]
    public Task<IActionResult> GetAvailable(
        [FromRoute] DateOnly day,
        [FromRoute, Range(1, int.MaxValue)] int appointmentTypeId,
        [FromQuery, Range(5, 240)] int? slotStepMinutes,
        CancellationToken ct)
        => _svc.GetAvailableStartTimesAsync(
                new GetAvailableStartTimesRequestDto(
                    Day: day,
                    AppointmentTypeId: new AppointmentTypeId(appointmentTypeId),
                    SlotStepMinutes: slotStepMinutes ?? 30),
                ct)
            .ToActionResultAsync(HttpContext);

    // POST /api/scheduling/appointments
    [HttpPost("appointments")]
    public Task<IActionResult> Create([FromBody] AppointmentUpsertRequestDto dto, CancellationToken ct)
        => _svc.CreateAppointmentAsync(dto, ct).ToActionResultAsync(HttpContext);

    // PUT /api/scheduling/appointments/123   (route id wins)
    [HttpPut("appointments/{id:int}")]
    public Task<IActionResult> Update(
        [FromRoute, Range(1, int.MaxValue)] int id,
        [FromBody] AppointmentUpsertRequestDto dto,
        CancellationToken ct)
    {
        if (dto.Id is not null && dto.Id.Value.Value != id)
        {
            var bad = ResultDto<bool>.Fail(new ErrorDto("validation", "Route id does not match body id."));
            return Task.FromResult(bad.ToActionResult(HttpContext));
        }

        dto = dto with { Id = new AppointmentId(id) };
        return _svc.UpdateAppointmentAsync(dto, ct).ToActionResultAsync(HttpContext);
    }

    // DELETE /api/scheduling/appointments/123
    [HttpDelete("appointments/{id:int}")]
    public Task<IActionResult> Delete([FromRoute, Range(1, int.MaxValue)] int id, CancellationToken ct)
=> _svc.DeleteAppointmentAsync(new AppointmentId(id), ct).ToActionResultAsync(HttpContext);

    public sealed record AppointmentResolveBodyDto([Required] DateTime ActualStartUtc);

    // POST /api/scheduling/appointments/123/resolve
    [HttpPost("appointments/{id:int}/resolve")]
    public Task<IActionResult> Resolve(
        [FromRoute, Range(1, int.MaxValue)] int id,
        [FromBody] AppointmentResolveBodyDto body,
        CancellationToken ct)
        => _svc.ResolveAppointmentAsync(
                new AppointmentResolveRequestDto(new AppointmentId(id), body.ActualStartUtc),
                ct)
            .ToActionResultAsync(HttpContext);

    // PUT /api/scheduling/day/2025-12-27/meta   (route day wins)
    [HttpPut("day/{day}/meta")]
    public Task<IActionResult> UpsertDayMeta(
        [FromRoute] DateOnly day,
        [FromBody] CalendarDayMetaUpsertDto dto,
        CancellationToken ct)
    {
        dto = dto with { Day = day };
        return _svc.UpsertCalendarDayMetaAsync(dto, ct).ToActionResultAsync(HttpContext);
    }
}
