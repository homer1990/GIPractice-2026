using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using GIPractice.Api.Models;

namespace GIPractice.Wpf.Backend.Queries.Patients;

public sealed class SavePatientQuery : IBackendQuery<PatientDto>
{
    private readonly PatientDto _patient;

    public SavePatientQuery(PatientDto patient)
    {
        _patient = patient ?? throw new ArgumentNullException(nameof(patient));
    }

    public async Task<PatientDto> ExecuteAsync(
        BackendContext context,
        CancellationToken cancellationToken)
    {
        // ASSUMPTION:
        //  - PatientDto has an int Id property (0 => new).
        //  - POST /api/patients creates, PUT /api/patients/{id} updates.
        // Adjust to your actual API if needed.

        HttpResponseMessage response;

        if (_patient.Id == 0)
        {
            response = await context.HttpClient.PostAsJsonAsync(
                "api/patients",
                _patient,
                cancellationToken);
        }
        else
        {
            response = await context.HttpClient.PutAsJsonAsync(
                $"api/patients/{_patient.Id}",
                _patient,
                cancellationToken);
        }

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PatientDto>(
            cancellationToken: cancellationToken);

        if (result is null)
            throw new InvalidOperationException("Patient save returned an empty body.");

        return result;
    }
}
