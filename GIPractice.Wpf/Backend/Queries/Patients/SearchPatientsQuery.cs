using System;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using GIPractice.Api.Models;

namespace GIPractice.Wpf.Backend.Queries;

public sealed class SearchPatientsQuery : IBackendQuery<PagedResultDto<PatientSummaryDto>>
{
    private readonly PatientSearchRequestDto _request;

    public SearchPatientsQuery(PatientSearchRequestDto request)
    {
        _request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public async Task<PagedResultDto<PatientSummaryDto>> ExecuteAsync(
        BackendContext context,
        CancellationToken cancellationToken)
    {
        // Adjust the URI if your endpoint is different
        using var response = await context.HttpClient.PostAsJsonAsync(
            "api/patients/search",
            _request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PagedResultDto<PatientSummaryDto>>(
            cancellationToken: cancellationToken);

        if (result is null)
            throw new InvalidOperationException("Patients search returned an empty body.");

        return result;
    }
}
