namespace GIPractice.Client;

/// <summary>
/// Transport boundary used by typed feature clients. Feature contracts are added only
/// when their DTOs are finalized; there are intentionally no object-typed placeholders.
/// </summary>
public interface IGiPracticeApiClient
{
    Task<TResponse> GetAsync<TResponse>(
        string relativeUri,
        CancellationToken cancellationToken = default);

    Task<TResponse> SendAsync<TRequest, TResponse>(
        HttpMethod method,
        string relativeUri,
        TRequest request,
        CancellationToken cancellationToken = default);

    Task SendAsync<TRequest>(
        HttpMethod method,
        string relativeUri,
        TRequest request,
        CancellationToken cancellationToken = default);
}
