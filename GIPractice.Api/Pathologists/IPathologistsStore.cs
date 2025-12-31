using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathologists;
using GIPractice.Contracts.Pathology;

namespace GIPractice.Api.Pathologists;

public interface IPathologistsStore
{
    Task<ResultDto<PagedResultDto<PathologistListItemDto>>> SearchAsync(PathologistSearchRequestDto request, CancellationToken ct = default);
    Task<ResultDto<PathologistDto>> GetAsync(PathologistId id, CancellationToken ct = default);
    Task<ResultDto<PathologistId>> CreateAsync(PathologistUpsertRequestDto request, CancellationToken ct = default);
    Task<ResultDto<bool>> UpdateAsync(PathologistUpsertRequestDto request, CancellationToken ct = default);
    Task<ResultDto<bool>> DeleteAsync(PathologistId id, CancellationToken ct = default);
}
