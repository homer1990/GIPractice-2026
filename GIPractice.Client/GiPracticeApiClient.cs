using System.Net;
using System.Net.Http.Json;

namespace GIPractice.Client;

public sealed class GiPracticeApiClient(HttpClient httpClient) : IGiPracticeApiClient
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<TResponse> GetAsync<TResponse>(
        string relativeUri,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(relativeUri, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken)
            ?? throw new ApiClientException(
                response.StatusCode,
                "The API returned an empty response body.");
    }

    public async Task<TResponse> SendAsync<TRequest, TResponse>(
        HttpMethod method,
        string relativeUri,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(method, relativeUri)
        {
            Content = JsonContent.Create(request)
        };
        using var response = await _httpClient.SendAsync(message, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken)
            ?? throw new ApiClientException(
                response.StatusCode,
                "The API returned an empty response body.");
    }

    public async Task SendAsync<TRequest>(
        HttpMethod method,
        string relativeUri,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        using var message = new HttpRequestMessage(method, relativeUri)
        {
            Content = JsonContent.Create(request)
        };
        using var response = await _httpClient.SendAsync(message, cancellationToken);
        await EnsureSuccessAsync(response, cancellationToken);
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return;

        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        throw new ApiClientException(response.StatusCode, body);
    }
}

public sealed class ApiClientException(HttpStatusCode statusCode, string responseBody)
    : HttpRequestException($"GIPractice API returned {(int)statusCode} ({statusCode}).")
{
    public HttpStatusCode StatusCodeValue { get; } = statusCode;
    public string ResponseBody { get; } = responseBody;
}
