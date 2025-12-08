using System.Threading;
using System.Threading.Tasks;
using GIPractice.Api.Models;

namespace GIPractice.Client;

public interface IPatientsModule
{
    Task<PagedResultDto<PatientListItemDto>> SearchAsync(
        PatientSearchRequestDto request,
        CancellationToken cancellationToken = default);

    Task<PatientDashboardDto?> GetDashboardAsync(
        int patientId,
        CancellationToken cancellationToken = default);
}

public class PatientsModule(GiPracticeApiClient api) : IPatientsModule
{
    private readonly GiPracticeApiClient _api = api;

    public Task<PagedResultDto<PatientListItemDto>> SearchAsync(
        PatientSearchRequestDto request,
        CancellationToken cancellationToken = default)
        => _api.SearchPatientsAsync(request, cancellationToken);

    public Task<PatientDashboardDto?> GetDashboardAsync(
        int patientId,
        CancellationToken cancellationToken = default)
        => _api.GetPatientDashboardAsync(patientId, cancellationToken);
}
