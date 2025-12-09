using System.Net.Http;

namespace GIPractice.Wpf.Backend;

/// <summary>
/// Shared context for backend queries (HttpClient, config, etc.).
/// </summary>
public sealed class BackendContext(HttpClient httpClient)
{
    public HttpClient HttpClient { get; } = httpClient;

    // Later: JWT/token provider, Json options, base path config, etc.
}
