using System.ComponentModel.DataAnnotations;
using GIPractice.Api.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Patients;
using Microsoft.AspNetCore.Mvc;

namespace GIPractice.Api.Patients;

[ApiController]
[Route("api/patients")]
public sealed class PatientsController(IPatientsService svc) : ControllerBase
{
    private readonly IPatientsService _svc = svc;

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] PatientSearchRequestDto request, CancellationToken ct)
        => Ok(await _svc.SearchAsync(request, ct));

    [HttpGet("{id:int}")]
    public Task<IActionResult> Get([FromRoute, Range(1, int.MaxValue)] int id, CancellationToken ct)
        => _svc.GetDetailsAsync(new PatientId(id), ct).ToActionResultAsync(HttpContext);

    [HttpPost]
    public Task<IActionResult> Create([FromBody] PatientUpsertRequestDto dto, CancellationToken ct)
        => _svc.CreateAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpPut("{id:int}")]
    public Task<IActionResult> Update(
        [FromRoute, Range(1, int.MaxValue)] int id,
        [FromBody] PatientUpsertRequestDto dto,
        CancellationToken ct)
    {
        dto = dto with { Id = new PatientId(id) };
        return _svc.UpdateAsync(dto, ct).ToActionResultAsync(HttpContext);
    }

    [HttpDelete("{id:int}")]
    public Task<IActionResult> Delete([FromRoute, Range(1, int.MaxValue)] int id, CancellationToken ct)
=> _svc.DeleteAsync(new PatientId(id), ct).ToActionResultAsync(HttpContext);
}
