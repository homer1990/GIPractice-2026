using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using GIPractice.Api.Models;

namespace GIPractice.Wpf.Backend;

public sealed class GiPracticeApiClient
{
    private readonly HttpClient _http;

    public GiPracticeApiClient(HttpClient http)
    {
        _http = http;
    }

    public string? BaseAddress
    {
        get => _http.BaseAddress?.ToString();
        set => _http.BaseAddress = value is null ? null : new Uri(value);
    }

    public async Task<PagedResultDto<PatientListItemDto>> SearchPatientsAsync(
        PatientSearchRequestDto request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsJsonAsync(
            "api/patients/search", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"SearchPatients failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<PatientListItemDto>>(
            cancellationToken: cancellationToken);

        return result ?? new PagedResultDto<PatientListItemDto>
        {
            Page = request.PageIndex,
            PageSize = request.PageSize,
            TotalCount = 0,
            Items = Array.Empty<PatientListItemDto>()
        };
    }

    public Task<PatientDashboardDto?> GetPatientDashboardAsync(
        int patientId,
        CancellationToken cancellationToken = default)
        => _http.GetFromJsonAsync<PatientDashboardDto>(
            $"api/patients/{patientId}/dashboard", cancellationToken);
}
