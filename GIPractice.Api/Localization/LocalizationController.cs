using GIPractice.Contracts.Common;
using GIPractice.Contracts.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GIPractice.Api.Common;

namespace GIPractice.Api.Localization;

[ApiController]
[Route("api/localization")]
public sealed class LocalizationController : ControllerBase
{
    private readonly ILocalizationService _svc;

    public LocalizationController(ILocalizationService svc) => _svc = svc;

    [HttpPost("search")]
    public Task<IActionResult> Search([FromBody] TranslationSearchRequestDto dto, CancellationToken ct)
        => _svc.SearchAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpGet]
    public Task<IActionResult> Get(
        [FromQuery] string key,
        [FromQuery] string culture,
        CancellationToken ct)
        => _svc.GetAsync(key, culture, ct).ToActionResultAsync(HttpContext);

    [HttpPut]
    public Task<IActionResult> Upsert([FromBody] TranslationUpsertRequestDto dto, CancellationToken ct)
        => _svc.UpsertAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpDelete]
    public Task<IActionResult> Delete(
        [FromQuery] string key,
        [FromQuery] string culture,
        CancellationToken ct)
        => _svc.DeleteAsync(key, culture, ct).ToActionResultAsync(HttpContext);
}
