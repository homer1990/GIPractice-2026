using GIPractice.Contracts.Patients;
using Microsoft.AspNetCore.Mvc;

namespace GIPractice.Api.Patients;

[ApiController]
[Route("api/patients")]
public sealed class PatientsController : ControllerBase
{
    private readonly IPatientsService _svc;

    public PatientsController(IPatientsService svc) => _svc = svc;

    [HttpPost("search")]
    public Task<IActionResult> Search([FromBody] PatientSearchRequestDto dto, CancellationToken ct)
        => Wrap(_svc.SearchAsync(dto, ct));

    [HttpGet("{id:int}/details")]
    public Task<IActionResult> GetDetails([FromRoute] int id, CancellationToken ct)
        => Wrap(_svc.GetDetailsAsync(new(id), ct));

    [HttpPost]
    public Task<IActionResult> Create([FromBody] PatientUpsertRequestDto dto, CancellationToken ct)
        => Wrap(_svc.CreateAsync(dto, ct));

    [HttpPut]
    public Task<IActionResult> Update([FromBody] PatientUpsertRequestDto dto, CancellationToken ct)
        => Wrap(_svc.UpdateAsync(dto, ct));

    [HttpDelete("{id:int}")]
    public Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        => Wrap(_svc.DeleteAsync(new(id), ct));

    private static async Task<IActionResult> Wrap<T>(Task<GIPractice.Contracts.Common.ResultDto<T>> task)
    {
        var res = await task;

        if (res.IsSuccess)
            return new OkObjectResult(res);

        var code = res.Error?.Code;
        return code switch
        {
            "not_found" => new NotFoundObjectResult(res),
            "conflict" => new ConflictObjectResult(res),
            "unauthorized" => new UnauthorizedObjectResult(res),
            "forbidden" => new ObjectResult(res) { StatusCode = StatusCodes.Status403Forbidden },
            _ => new BadRequestObjectResult(res)
        };
    }
}
