using GIPractice.Contracts.Common;
using GIPractice.Contracts.Encounters;
using GIPractice.Contracts.Ids;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GIPractice.Api.Common;

namespace GIPractice.Api.Encounters;

[ApiController]
[Route("api/encounters")]
public sealed class EncountersController : ControllerBase
{
    private readonly IEncountersService _svc;

    public EncountersController(IEncountersService svc) => _svc = svc;

    [HttpPost("search")]
    public Task<IActionResult> Search([FromBody] EncounterSearchRequestDto dto, CancellationToken ct)
        => _svc.SearchAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpGet("{id:int}")]
    public Task<IActionResult> Get([FromRoute] int id, CancellationToken ct)
        => _svc.GetAsync(new EncounterId(id), ct).ToActionResultAsync(HttpContext);

    [HttpPost]
    public Task<IActionResult> Create([FromBody] EncounterUpsertRequestDto dto, CancellationToken ct)
        => _svc.CreateAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpPut]
    public Task<IActionResult> Update([FromBody] EncounterUpsertRequestDto dto, CancellationToken ct)
        => _svc.UpdateAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpDelete("{id:int}")]
    public Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        => _svc.DeleteAsync(new EncounterId(id), ct).ToActionResultAsync(HttpContext);
}
