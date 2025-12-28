using GIPractice.Contracts.Common;
using GIPractice.Contracts.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GIPractice.Api.Common;

namespace GIPractice.Api.Settings;

[ApiController]
[Route("api/settings")]
public sealed class SettingsController : ControllerBase
{
    private readonly ISettingsService _svc;

    public SettingsController(ISettingsService svc) => _svc = svc;

    [HttpPost("search")]
    public Task<IActionResult> Search([FromBody] AppSettingSearchRequestDto dto, CancellationToken ct)
        => _svc.SearchAsync(dto, ct).ToActionResultAsync(HttpContext);

    // GET with query is easiest for (key, scope, scopeKey) triple
    [HttpGet]
    public Task<IActionResult> Get(
        [FromQuery] string key,
        [FromQuery] AppSettingScope scope,
        [FromQuery] string? scopeKey,
        CancellationToken ct)
        => _svc.GetAsync(key, scope, scopeKey, ct).ToActionResultAsync(HttpContext);

    [HttpPut]
    public Task<IActionResult> Upsert([FromBody] AppSettingUpsertRequestDto dto, CancellationToken ct)
        => _svc.UpsertAsync(dto, ct).ToActionResultAsync(HttpContext);

    [HttpDelete]
    public Task<IActionResult> Delete(
        [FromQuery] string key,
        [FromQuery] AppSettingScope scope,
        [FromQuery] string? scopeKey,
        CancellationToken ct)
        => _svc.DeleteAsync(key, scope, scopeKey, ct).ToActionResultAsync(HttpContext);
}
