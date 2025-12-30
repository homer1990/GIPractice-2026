using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GIPractice.Api.Tests;

public static class DevSeedHelper
{
    public static async Task SeedAsync(HttpClient http, CancellationToken ct = default)
    {
        // Dev-only endpoint. It's safe to call multiple times.
        var resp = await http.PostAsync("/api/dev/seed", content: null, ct);
        resp.EnsureSuccessStatusCode();
    }
}
