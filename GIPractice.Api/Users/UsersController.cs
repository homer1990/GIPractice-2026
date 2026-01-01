using GIPractice.Api.Common;
using GIPractice.Contracts.Auth;
using GIPractice.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GIPractice.Api.Users;

[ApiController]
[Route("api/users")]
[Authorize(Roles = AppRoles.Admin)]
public sealed class UsersController(IUsersService svc) : ControllerBase
{
    [HttpPost("search")]
    public Task<IActionResult> Search([FromBody] UserSearchRequestDto request, CancellationToken cancellationToken = default)
        => svc.SearchAsync(request, cancellationToken).ToActionResultAsync(HttpContext);

    [HttpGet("{id}")]
    public Task<IActionResult> Get([FromRoute] string id, CancellationToken cancellationToken = default)
        => svc.GetAsync(id, cancellationToken).ToActionResultAsync(HttpContext);

    [HttpPost]
    public Task<IActionResult> Create([FromBody] UserCreateRequestDto request, CancellationToken cancellationToken = default)
        => svc.CreateAsync(request, cancellationToken).ToActionResultAsync(HttpContext);

    [HttpPut("{id}")]
    public Task<IActionResult> Update([FromRoute] string id, [FromBody] UserUpdateRequestDto request, CancellationToken cancellationToken = default)
        => svc.UpdateAsync(id, request, cancellationToken).ToActionResultAsync(HttpContext);

    [HttpPost("{id}/password")]
    public Task<IActionResult> SetPassword([FromRoute] string id, [FromBody] UserSetPasswordRequestDto request, CancellationToken cancellationToken = default)
        => svc.SetPasswordAsync(id, request, cancellationToken).ToActionResultAsync(HttpContext);

    [HttpDelete("{id}")]
    public Task<IActionResult> Delete([FromRoute] string id, CancellationToken cancellationToken = default)
        => svc.DeleteAsync(id, cancellationToken).ToActionResultAsync(HttpContext);
}
