using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Patients;

namespace GIPractice.Api.Patients;

public sealed class PatientsService : IPatientsService
{
    private readonly IPatientsStore _store;

    public PatientsService(IPatientsStore store)
    {
        _store = store;
    }

    public Task<ResultDto<PagedResultDto<PatientListItemDto>>> SearchAsync(
        PatientSearchRequestDto request,
        CancellationToken cancellationToken = default)
        => _store.SearchAsync(request, cancellationToken);

    public Task<ResultDto<PatientDetailsDto>> GetDetailsAsync(
        PatientId patientId,
        CancellationToken cancellationToken = default)
        => _store.GetDetailsAsync(patientId, cancellationToken);

    public Task<ResultDto<PatientId>> CreateAsync(
        PatientUpsertRequestDto request,
        CancellationToken cancellationToken = default)
        => _store.CreateAsync(request, cancellationToken);

    public Task<ResultDto<bool>> UpdateAsync(
        PatientUpsertRequestDto request,
        CancellationToken cancellationToken = default)
        => _store.UpdateAsync(request, cancellationToken);

    public Task<ResultDto<bool>> DeleteAsync(
        PatientId patientId,
        CancellationToken cancellationToken = default)
        => _store.DeleteAsync(patientId, cancellationToken);
}
