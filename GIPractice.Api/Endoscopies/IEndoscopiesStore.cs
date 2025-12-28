using GIPractice.Contracts.Common;
using GIPractice.Contracts.Endoscopies;
using GIPractice.Contracts.Ids;

namespace GIPractice.Api.Endoscopies;

public interface IEndoscopiesStore
{
    Task<PagedResultDto<EndoscopyListItemDto>> SearchAsync(EndoscopySearchRequestDto request, CancellationToken ct);
    Task<EndoscopyDetailsDto?> GetAsync(EndoscopyId id, CancellationToken ct);
    Task<ResultDto<EndoscopyId>> CreateAsync(EndoscopyUpsertRequestDto request, CancellationToken ct);
    Task<ResultDto<bool>> UpdateAsync(EndoscopyUpsertRequestDto request, CancellationToken ct);
    Task<ResultDto<bool>> DeleteAsync(EndoscopyId id, CancellationToken ct);
}
