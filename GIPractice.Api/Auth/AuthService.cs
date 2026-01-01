using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GIPractice.Contracts.Auth;
using GIPractice.Contracts.Common;
using GIPractice.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GIPractice.Api.Auth;

public sealed class AuthService(
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor,
    IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IHttpContextAccessor _http = httpContextAccessor;
    private readonly JwtOptions _jwt = jwtOptions.Value;

    public async Task<ResultDto<LoginResponseDto>> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            return ResultDto<LoginResponseDto>.Fail("validation", "Username and password are required.");

        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user is null)
            return ResultDto<LoginResponseDto>.Fail("unauthorized", "Invalid credentials.");

        if (!user.IsActive)
            return ResultDto<LoginResponseDto>.Fail("unauthorized", "User is inactive.");

        var ok = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!ok)
            return ResultDto<LoginResponseDto>.Fail("unauthorized", "Invalid credentials.");

        var roles = await _userManager.GetRolesAsync(user);

        var expires = DateTime.UtcNow.AddMinutes(_jwt.AccessTokenMinutes);
        var token = CreateAccessToken(user, roles, expires);

        user.LastLoginAtUtc = DateTimeOffset.UtcNow;
        await _userManager.UpdateAsync(user);

        var me = new AuthMeDto(
            UserId: user.Id,
            UserName: user.UserName ?? "",
            DisplayName: user.DisplayName,
            Roles: roles.ToArray(),
            IsActive: user.IsActive);

        return ResultDto<LoginResponseDto>.Ok(new LoginResponseDto(token, expires, me));
    }

    public async Task<ResultDto<AuthMeDto>> MeAsync(CancellationToken cancellationToken = default)
    {
        var principal = _http.HttpContext?.User;
        if (principal is null || principal.Identity?.IsAuthenticated != true)
            return ResultDto<AuthMeDto>.Fail("unauthorized", "Not authenticated.");

        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrWhiteSpace(userId))
            return ResultDto<AuthMeDto>.Fail("unauthorized", "Missing user id claim.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return ResultDto<AuthMeDto>.Fail("unauthorized", "User not found.");

        var roles = await _userManager.GetRolesAsync(user);

        return ResultDto<AuthMeDto>.Ok(new AuthMeDto(
            UserId: user.Id,
            UserName: user.UserName ?? "",
            DisplayName: user.DisplayName,
            Roles: roles.ToArray(),
            IsActive: user.IsActive));
    }

    private string CreateAccessToken(ApplicationUser user, IEnumerable<string> roles, DateTime expiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(_jwt.Key))
            throw new InvalidOperationException("JWT key not configured (Jwt:Key).");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? ""),
            new("display_name", user.DisplayName ?? ""),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAtUtc,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
