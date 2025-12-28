using GIPractice.Contracts.Biopsies;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GIPractice.Api.Common;

namespace GIPractice.Api.Biopsies;

[ApiController]
[Route("api/biopsies")]
public sealed class BiopsiesController : ControllerBase
{
    private readonly IBiopsiesService _svc;

    public BiopsiesController(IBiopsiesService svc) => _svc = svc;

    // Bottles
    [HttpPost("bottles/search")]
    public Task<IActionResult> SearchBottles([FromBody] BiopsyBottleSearchRequestDto dto, CancellationToken ct)
        => _svc.SearchBottlesAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpGet("bottles/{id:int}")]
    public Task<IActionResult> GetBottle([FromRoute] int id, CancellationToken ct)
        => _svc.GetBottleAsync(new BiopsyBottleId(id), ct).ToActionResultAsync(HttpContext);

    [HttpPost("bottles")]
    public Task<IActionResult> CreateBottle([FromBody] BiopsyBottleUpsertRequestDto dto, CancellationToken ct)
        => _svc.CreateBottleAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpPut("bottles")]
    public Task<IActionResult> UpdateBottle([FromBody] BiopsyBottleUpsertRequestDto dto, CancellationToken ct)
        => _svc.UpdateBottleAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpDelete("bottles/{id:int}")]
    public Task<IActionResult> DeleteBottle([FromRoute] int id, CancellationToken ct)
        => _svc.DeleteBottleAsync(new BiopsyBottleId(id), ct).ToActionResultAsync(HttpContext);

    // Dispatch bundles
    [HttpPost("dispatch/search")]
    public Task<IActionResult> SearchDispatch([FromBody] BiopsyDispatchSearchRequestDto dto, CancellationToken ct)
        => _svc.SearchDispatchBundlesAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpGet("dispatch/{id:int}")]
    public Task<IActionResult> GetDispatch([FromRoute] int id, CancellationToken ct)
        => _svc.GetDispatchDetailsAsync(new BiopsyDispatchBundleId(id), ct).ToActionResultAsync(HttpContext);

    [HttpPost("dispatch")]
    public Task<IActionResult> CreateDispatch([FromBody] BiopsyDispatchCreateRequestDto dto, CancellationToken ct)
        => _svc.CreateDispatchBundleAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpPut("dispatch/close")]
    public Task<IActionResult> CloseDispatch([FromBody] BiopsyDispatchCloseRequestDto dto, CancellationToken ct)
        => _svc.CloseDispatchBundleAsync(dto, ct).ToActionResultAsync(HttpContext);
}
