using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Pathologists;

public interface IPathologistsService
{
    Task<ResultDto<PagedResultDto<PathologistListItemDto>>> SearchAsync(
        PathologistSearchRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<PathologistDto>> GetAsync(
        PathologistId id,
        CancellationToken cancellationToken = default);

    Task<ResultDto<PathologistId>> CreateAsync(
        PathologistUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> UpdateAsync(
        PathologistUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> DeleteAsync(
        PathologistId id,
        CancellationToken cancellationToken = default);
}
