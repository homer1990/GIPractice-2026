using System;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using GIPractice.Api.Models;
#pragma warning disable IDE0130
namespace GIPractice.Wpf.Backend.Queries;
#pragma warning restore IDE0130
public sealed class SearchPatientsQuery(PatientSearchRequestDto request) : IBackendQuery<PagedResultDto<PatientSummaryDto>>
{
    private readonly PatientSearchRequestDto _request = request ?? throw new ArgumentNullException(nameof(request));

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

        return result is null ? throw new InvalidOperationException("Patients search returned an empty body.") : result;
    }
}
