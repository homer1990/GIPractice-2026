using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Endoscopies;

public interface IEndoscopiesService
{
    Task<ResultDto<PagedResultDto<EndoscopyListItemDto>>> SearchAsync(
        EndoscopySearchRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<EndoscopyDetailsDto>> GetAsync(
        EndoscopyId id,
        CancellationToken cancellationToken = default);

    Task<ResultDto<EndoscopyId>> CreateAsync(
        EndoscopyUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> UpdateAsync(
        EndoscopyUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> DeleteAsync(
        EndoscopyId id,
        CancellationToken cancellationToken = default);
}
