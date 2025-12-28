using GIPractice.Contracts.Patients;
using Microsoft.AspNetCore.Mvc;
using GIPractice.Api.Common;

namespace GIPractice.Api.Patients;

[ApiController]
[Route("api/patients")]
public sealed class PatientsController : ControllerBase
{
    private readonly IPatientsService _svc;

    public PatientsController(IPatientsService svc) => _svc = svc;

    [HttpPost("search")]
    public Task<IActionResult> Search([FromBody] PatientSearchRequestDto dto, CancellationToken ct)
        => _svc.SearchAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpGet("{id:int}/details")]
    public Task<IActionResult> GetDetails([FromRoute] int id, CancellationToken ct)
        => _svc.GetDetailsAsync(new(id), ct).ToActionResultAsync(HttpContext);

    [HttpPost]
    public Task<IActionResult> Create([FromBody] PatientUpsertRequestDto dto, CancellationToken ct)
        => _svc.CreateAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpPut]
    public Task<IActionResult> Update([FromBody] PatientUpsertRequestDto dto, CancellationToken ct)
        => _svc.UpdateAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpDelete("{id:int}")]
    public Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        => _svc.DeleteAsync(new(id), ct).ToActionResultAsync(HttpContext);
}
