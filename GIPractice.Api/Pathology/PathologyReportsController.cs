using GIPractice.Api.Auth;
using GIPractice.Api.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GIPractice.Api.Pathology;

[ApiController]
[Route("api/pathology/reports")]
[Authorize(Roles = RoleSets.Clinician)]
public sealed class PathologyReportsController(IPathologyReportsService svc) : ControllerBase
{
    [HttpPost("search")]
    public Task<IActionResult> Search([FromBody] PathologyReportSearchRequestDto dto, CancellationToken ct)
        => svc.SearchAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpGet("{id:int:min(1)}")]
    public Task<IActionResult> Get(int id, CancellationToken ct)
        => svc.GetAsync(new PathologyReportId(id), ct).ToActionResultAsync(HttpContext);

    [HttpPost]
    public Task<IActionResult> Create([FromBody] PathologyReportUpsertRequestDto dto, CancellationToken ct)
        => svc.CreateAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpPut("{id:int:min(1)}")]
    public Task<IActionResult> Update(int id, [FromBody] PathologyReportUpsertRequestDto dto, CancellationToken ct)
    {
        dto = dto with { Id = new PathologyReportId(id) };
        return svc.UpdateAsync(dto, ct).ToActionResultAsync(HttpContext);
    }

    [HttpDelete("{id:int:min(1)}")]
    public Task<IActionResult> Delete(int id, CancellationToken ct)
        => svc.DeleteAsync(new PathologyReportId(id), ct).ToActionResultAsync(HttpContext);
}
