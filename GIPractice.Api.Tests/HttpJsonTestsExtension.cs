using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GIPractice.Api.Tests;

public static class HttpJsonTestExtensions
{
    public static Task<HttpResponseMessage> PostJsonAsync<T>(
        this HttpClient http,
        string requestUri,
        T value,
        CancellationToken cancellationToken = default)
        => http.PostAsJsonAsync(requestUri, value, TestJson.Options, cancellationToken);

    public static Task<HttpResponseMessage> PutJsonAsync<T>(
        this HttpClient http,
        string requestUri,
        T value,
        CancellationToken cancellationToken = default)
        => http.PutAsJsonAsync(requestUri, value, TestJson.Options, cancellationToken);

    public static Task<T?> ReadJsonAsync<T>(
        this HttpContent content,
        CancellationToken cancellationToken = default)
        => content.ReadFromJsonAsync<T>(TestJson.Options, cancellationToken);
}