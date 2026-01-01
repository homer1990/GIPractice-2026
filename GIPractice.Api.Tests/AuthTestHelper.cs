using System.Net;
using System.Net.Http.Headers;
using GIPractice.Contracts.Auth;
using GIPractice.Contracts.Common;

namespace GIPractice.Api.Tests;

public static class AuthTestHelper
{
    public static async Task AuthenticateAsAdminAsync(HttpClient http)
    {
        // already set
        if (http.DefaultRequestHeaders.Authorization?.Scheme == "Bearer")
            return;

        var login = new LoginRequestDto(UserName: "admin", Password: "admin");
        var resp = await http.PostJsonAsync("/api/auth/login", login);

        if (resp.StatusCode != HttpStatusCode.OK)
            throw new InvalidOperationException($"Login failed: {(int)resp.StatusCode} {resp.StatusCode}");

        var body = await resp.Content.ReadJsonAsync<ResultDto<LoginResponseDto>>();
        if (body is null || !body.IsSuccess || body.Value is null)
            throw new InvalidOperationException($"Login failed: {body?.Error?.Code} {body?.Error?.Message}");

        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", body.Value.AccessToken);
    }
}
