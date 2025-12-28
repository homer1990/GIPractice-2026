using GIPractice.Contracts.Common;
using GIPractice.Contracts.Endoscopies;
using GIPractice.Contracts.Ids;

namespace GIPractice.Api.Endoscopies;

public sealed class EndoscopiesService : IEndoscopiesService
{
    private readonly IEndoscopiesStore _store;

    public EndoscopiesService(IEndoscopiesStore store) => _store = store;

    public async Task<ResultDto<PagedResultDto<EndoscopyListItemDto>>> SearchAsync(EndoscopySearchRequestDto request, CancellationToken cancellationToken = default)
        => ResultDto<PagedResultDto<EndoscopyListItemDto>>.Ok(await _store.SearchAsync(request, cancellationToken));

    public async Task<ResultDto<EndoscopyDetailsDto>> GetAsync(EndoscopyId id, CancellationToken cancellationToken = default)
    {
        var item = await _store.GetAsync(id, cancellationToken);
        return item is null
            ? ResultDto<EndoscopyDetailsDto>.Fail("not_found", "Endoscopy not found.")
            : ResultDto<EndoscopyDetailsDto>.Ok(item);
    }

    public Task<ResultDto<EndoscopyId>> CreateAsync(EndoscopyUpsertRequestDto request, CancellationToken cancellationToken = default)
        => _store.CreateAsync(request, cancellationToken);

    public Task<ResultDto<bool>> UpdateAsync(EndoscopyUpsertRequestDto request, CancellationToken cancellationToken = default)
        => _store.UpdateAsync(request, cancellationToken);

    public Task<ResultDto<bool>> DeleteAsync(EndoscopyId id, CancellationToken cancellationToken = default)
        => _store.DeleteAsync(id, cancellationToken);
}
