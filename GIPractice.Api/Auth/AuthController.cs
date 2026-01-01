using GIPractice.Api.Common;
using GIPractice.Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GIPractice.Api.Auth;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService svc) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public Task<IActionResult> Login([FromBody] LoginRequestDto dto, CancellationToken cancellationToken = default)
        => svc.LoginAsync(dto, cancellationToken).ToActionResultAsync(HttpContext);

    [HttpGet("me")]
    [Authorize]
    public Task<IActionResult> Me(CancellationToken cancellationToken = default)
        => svc.MeAsync(cancellationToken).ToActionResultAsync(HttpContext);
}
