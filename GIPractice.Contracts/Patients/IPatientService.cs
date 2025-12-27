using GIPractice.Contracts.Common;

namespace GIPractice.Contracts.Patients;

public interface IPatientsService
{
    Task<PagedResultDto<PatientListItemDto>> SearchAsync(
        PatientSearchRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<PatientDetailsDto>> GetDetailsAsync(
        int patientId,
        CancellationToken cancellationToken = default);

    Task<ResultDto<int>> CreateAsync(
        PatientUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> UpdateAsync(
        PatientUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> DeleteAsync(
        int patientId,
        CancellationToken cancellationToken = default);
}
