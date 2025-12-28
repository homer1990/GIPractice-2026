using System.Threading;
using System.Threading.Tasks;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Patients;

public interface IPatientsService
{
    Task<ResultDto<PagedResultDto<PatientListItemDto>>> SearchAsync(
        PatientSearchRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<PatientDetailsDto>> GetDetailsAsync(
        PatientId patientId,
        CancellationToken cancellationToken = default);

    Task<ResultDto<PatientId>> CreateAsync(
        PatientUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> UpdateAsync(
        PatientUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> DeleteAsync(
        PatientId patientId,
        CancellationToken cancellationToken = default);
}
