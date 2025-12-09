using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using GIPractice.Api.Models;

namespace GIPractice.Wpf.Backend.Queries;

public sealed class SearchPatientsQuery : IBackendQuery<PagedResultDto<PatientListItemDto>>
{
    private readonly PatientSearchRequestDto _request;

    public SearchPatientsQuery(PatientSearchRequestDto request)
    {
        _request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public async Task<PagedResultDto<PatientListItemDto>> ExecuteAsync(
        BackendContext context,
        CancellationToken cancellationToken)
    {
        // NOTE:
        // I don't have direct visibility of your API routes here.
        // I'm assuming a POST endpoint at: POST /api/patients/search
        // Adjust the path if your controller uses something else.

        using var response = await context.HttpClient.PostAsJsonAsync(
            "api/patients/search",
            _request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<PatientListItemDto>>(
            cancellationToken: cancellationToken);

        if (result is null)
            throw new InvalidOperationException("Patients search returned an empty body.");

        return result;
    }
}
