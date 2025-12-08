using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GIPractice.Api.Models;
using GIPractice.Api.Options;
using GIPractice.Core.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly JwtOptions _jwt = jwtOptions.Value;

    // POST /api/auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Username and password are required.");
        }

        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null)
        {
            await Task.Delay(Random.Shared.Next(50, 150));
            return Unauthorized("Invalid username or password.");
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user, request.Password, lockoutOnFailure: true);

        if (!signInResult.Succeeded)
            return Unauthorized("Invalid username or password.");

        var roles = await _userManager.GetRolesAsync(user);

        var nowUtc = DateTime.UtcNow;
        var expires = nowUtc.AddMinutes(_jwt.AccessTokenLifetimeMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty)
        };

        if (!string.IsNullOrWhiteSpace(user.Email))
            claims.Add(new Claim(ClaimTypes.Email, user.Email!));

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: nowUtc,
            expires: expires,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        var userDto = new CurrentUserDto
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            DisplayName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Roles = [.. roles]
        };

        var response = new LoginResponseDto
        {
            AccessToken = tokenString,
            ExpiresAtUtc = expires,
            User = userDto
        };

        return Ok(response);
    }

    // GET /api/auth/me
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<CurrentUserDto>> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);

        var dto = new CurrentUserDto
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            DisplayName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Roles = [.. roles]
        };

        return Ok(dto);
    }
}
