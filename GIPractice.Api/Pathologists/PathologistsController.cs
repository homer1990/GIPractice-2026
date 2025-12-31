using GIPractice.Api.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathologists;
using GIPractice.Contracts.Pathology;
using Microsoft.AspNetCore.Mvc;

namespace GIPractice.Api.Pathologists;

[ApiController]
[Route("api/pathologists")]
public sealed class PathologistsController(IPathologistsService svc) : ControllerBase
{
    [HttpPost("search")]
    public Task<IActionResult> Search([FromBody] PathologistSearchRequestDto dto, CancellationToken ct)
        => svc.SearchAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpGet("{id:int:min(1)}")]
    public Task<IActionResult> Get(int id, CancellationToken ct)
        => svc.GetAsync(new PathologistId(id), ct).ToActionResultAsync(HttpContext);

    [HttpPost]
    public Task<IActionResult> Create([FromBody] PathologistUpsertRequestDto dto, CancellationToken ct)
        => svc.CreateAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpPut("{id:int:min(1)}")]
    public Task<IActionResult> Update(int id, [FromBody] PathologistUpsertRequestDto dto, CancellationToken ct)
    {
        // Route wins (same behavior as Patients).
        dto = dto with { Id = new PathologistId(id) };
        return svc.UpdateAsync(dto, ct).ToActionResultAsync(HttpContext);
    }

    [HttpDelete("{id:int:min(1)}")]
    public Task<IActionResult> Delete(int id, CancellationToken ct)
        => svc.DeleteAsync(new PathologistId(id), ct).ToActionResultAsync(HttpContext);
}