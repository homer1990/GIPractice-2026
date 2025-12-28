using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Infai;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GIPractice.Api.Common;

namespace GIPractice.Api.Infai;

[ApiController]
[Route("api/infai")]
public sealed class InfaiController : ControllerBase
{
    private readonly IInfaiService _svc;

    public InfaiController(IInfaiService svc) => _svc = svc;

    [HttpPost("search")]
    public Task<IActionResult> Search([FromBody] InfaiReportSearchRequestDto dto, CancellationToken ct)
        => _svc.SearchAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpGet("{id:int}")]
    public Task<IActionResult> Get([FromRoute] int id, CancellationToken ct)
        => _svc.GetAsync(new InfaiReportId(id), ct).ToActionResultAsync(HttpContext);

    [HttpPost]
    public Task<IActionResult> Create([FromBody] InfaiReportUpsertRequestDto dto, CancellationToken ct)
        => _svc.CreateAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpPut]
    public Task<IActionResult> Update([FromBody] InfaiReportUpsertRequestDto dto, CancellationToken ct)
        => _svc.UpdateAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpDelete("{id:int}")]
    public Task<IActionResult> Delete([FromRoute] int id, CancellationToken ct)
        => _svc.DeleteAsync(new InfaiReportId(id), ct).ToActionResultAsync(HttpContext);
}
