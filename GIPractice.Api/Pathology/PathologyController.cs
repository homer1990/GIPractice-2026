using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GIPractice.Api.Common;

namespace GIPractice.Api.Pathology;

[ApiController]
[Route("api/pathology")]
public sealed class PathologyController : ControllerBase
{
    private readonly IPathologyService _svc;

    public PathologyController(IPathologyService svc) => _svc = svc;

    [HttpPost("search")]
    public Task<IActionResult> Search([FromBody] PathologyReportSearchRequestDto dto, CancellationToken ct)
        => _svc.SearchAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpGet("{id:int}")]
    public Task<IActionResult> Get([FromRoute] int id, CancellationToken ct)
        => _svc.GetAsync(new PathologyReportId(id), ct).ToActionResultAsync(HttpContext);

    [HttpPost]
    public Task<IActionResult> Create([FromBody] PathologyReportUpsertRequestDto dto, CancellationToken ct)
        => _svc.CreateAsync(dto, ct).ToActionResultAsync(HttpContext);

    // Route id always wins. Body Id may be null or wrong; we overwrite it.
    [HttpPut("{id:int}")]
    public Task<IActionResult> Update([FromRoute] int id, [FromBody] PathologyReportUpsertRequestDto dto, CancellationToken ct)
        => _svc.UpdateAsync(dto with { Id = new PathologyReportId(id) }, ct).ToActionResultAsync(HttpContext);

    [HttpDelete("{id:int}")]
    public Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        => _svc.DeleteAsync(new PathologyReportId(id), ct).ToActionResultAsync(HttpContext);
}
