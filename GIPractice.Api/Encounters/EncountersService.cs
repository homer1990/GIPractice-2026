using GIPractice.Contracts.Common;
using GIPractice.Contracts.Encounters;
using GIPractice.Contracts.Ids;

namespace GIPractice.Api.Encounters;

public sealed class EncountersService : IEncountersService
{
    private readonly IEncountersStore _store;

    public EncountersService(IEncountersStore store) => _store = store;

    public async Task<ResultDto<PagedResultDto<EncounterListItemDto>>> SearchAsync(EncounterSearchRequestDto request, CancellationToken cancellationToken = default)
        => ResultDto<PagedResultDto<EncounterListItemDto>>.Ok(await _store.SearchAsync(request, cancellationToken));

    public async Task<ResultDto<EncounterDetailsDto>> GetAsync(EncounterId id, CancellationToken cancellationToken = default)
    {
        var item = await _store.GetAsync(id, cancellationToken);
        return item is null
            ? ResultDto<EncounterDetailsDto>.Fail("not_found", "Encounter not found.")
            : ResultDto<EncounterDetailsDto>.Ok(item);
    }

    public Task<ResultDto<EncounterId>> CreateAsync(EncounterUpsertRequestDto request, CancellationToken cancellationToken = default)
        => _store.CreateAsync(request, cancellationToken);

    public Task<ResultDto<bool>> UpdateAsync(EncounterUpsertRequestDto request, CancellationToken cancellationToken = default)
        => _store.UpdateAsync(request, cancellationToken);

    public Task<ResultDto<bool>> DeleteAsync(EncounterId id, CancellationToken cancellationToken = default)
        => _store.DeleteAsync(id, cancellationToken);
}
