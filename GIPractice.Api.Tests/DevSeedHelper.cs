using System.Net;

namespace GIPractice.Api.Tests;

public static class DevSeedHelper
{
    public static async Task SeedAsync(HttpClient http, CancellationToken cancellationToken = default)
    {
        var resp = await http.PostAsync("/api/dev/seed", content: null, cancellationToken);
        if (resp.StatusCode == HttpStatusCode.NotFound)
            return;
        resp.EnsureSuccessStatusCode();
    }
}