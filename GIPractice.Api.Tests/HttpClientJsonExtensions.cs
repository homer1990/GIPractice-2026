using System.Net.Http.Json;

namespace GIPractice.Api.Tests;

public static class HttpClientJsonExtensions
{
    public static Task<HttpResponseMessage> PostJsonAsync<T>(this HttpClient http, string url, T body)
        => http.PostAsJsonAsync(url, body, TestJson.Options);

    public static Task<HttpResponseMessage> PutJsonAsync<T>(this HttpClient http, string url, T body)
        => http.PutAsJsonAsync(url, body, TestJson.Options);

    public static Task<T?> ReadJsonAsync<T>(this HttpContent content)
        => content.ReadFromJsonAsync<T>(TestJson.Options);
}
