using System;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using GIPractice.Api.Models;

namespace GIPractice.Wpf.Backend.Queries.Patients;

public sealed class GetPatientQuery : IBackendQuery<PatientDto>
{
    private readonly int _id;

    public GetPatientQuery(int id)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
        _id = id;
    }

    public async Task<PatientDto> ExecuteAsync(
        BackendContext context,
        CancellationToken cancellationToken)
    {
        // ASSUMPTION: GET /api/patients/{id}
        var dto = await context.HttpClient.GetFromJsonAsync<PatientDto>(
            $"api/patients/{_id}",
            cancellationToken);

        if (dto is null)
            throw new InvalidOperationException($"Patient {_id} not found.");

        return dto;
    }
}
