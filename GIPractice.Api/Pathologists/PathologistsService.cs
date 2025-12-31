using GIPractice.Contracts.Pathologists;
using GIPractice.Contracts.Pathology;

namespace GIPractice.Api.Pathologists;

public sealed class PathologistsService(IPathologistsStore store) : IPathologistsService
{
    public Task<GIPractice.Contracts.Common.ResultDto<GIPractice.Contracts.Common.PagedResultDto<PathologistListItemDto>>> SearchAsync(
        PathologistSearchRequestDto request, CancellationToken ct = default)
        => store.SearchAsync(request, ct);

    public Task<GIPractice.Contracts.Common.ResultDto<PathologistDto>> GetAsync(
        GIPractice.Contracts.Ids.PathologistId id, CancellationToken ct = default)
        => store.GetAsync(id, ct);

    public Task<GIPractice.Contracts.Common.ResultDto<GIPractice.Contracts.Ids.PathologistId>> CreateAsync(
        PathologistUpsertRequestDto request, CancellationToken ct = default)
        => store.CreateAsync(request, ct);

    public Task<GIPractice.Contracts.Common.ResultDto<bool>> UpdateAsync(
        PathologistUpsertRequestDto request, CancellationToken ct = default)
        => store.UpdateAsync(request, ct);

    public Task<GIPractice.Contracts.Common.ResultDto<bool>> DeleteAsync(
        GIPractice.Contracts.Ids.PathologistId id, CancellationToken ct = default)
        => store.DeleteAsync(id, ct);
}
