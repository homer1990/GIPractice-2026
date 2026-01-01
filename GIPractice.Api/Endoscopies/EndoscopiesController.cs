using GIPractice.Api.Auth;
using GIPractice.Api.Common;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Endoscopies;
using GIPractice.Contracts.Ids;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GIPractice.Api.Endoscopies;

[ApiController]
[Route("api/endoscopies")]
[Authorize(Roles = RoleSets.Staff)]
public sealed class EndoscopiesController : ControllerBase
{
    private readonly IEndoscopiesService _svc;

    public EndoscopiesController(IEndoscopiesService svc) => _svc = svc;

    [HttpPost("search")]
    public Task<IActionResult> Search([FromBody] EndoscopySearchRequestDto dto, CancellationToken ct)
        => _svc.SearchAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpGet("{id:int}")]
    public Task<IActionResult> Get([FromRoute] int id, CancellationToken ct)
        => _svc.GetAsync(new EndoscopyId(id), ct).ToActionResultAsync(HttpContext);

    [HttpPost]
    public Task<IActionResult> Create([FromBody] EndoscopyUpsertRequestDto dto, CancellationToken ct)
        => _svc.CreateAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpPut]
    public Task<IActionResult> Update([FromBody] EndoscopyUpsertRequestDto dto, CancellationToken ct)
        => _svc.UpdateAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpDelete("{id:int}")]
    public Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        => _svc.DeleteAsync(new EndoscopyId(id), ct).ToActionResultAsync(HttpContext);
}
