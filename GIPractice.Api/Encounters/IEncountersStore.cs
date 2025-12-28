using GIPractice.Contracts.Common;
using GIPractice.Contracts.Encounters;
using GIPractice.Contracts.Ids;

namespace GIPractice.Api.Encounters;

public interface IEncountersStore
{
    Task<PagedResultDto<EncounterListItemDto>> SearchAsync(EncounterSearchRequestDto request, CancellationToken ct);
    Task<EncounterDetailsDto?> GetAsync(EncounterId id, CancellationToken ct);
    Task<ResultDto<EncounterId>> CreateAsync(EncounterUpsertRequestDto request, CancellationToken ct);
    Task<ResultDto<bool>> UpdateAsync(EncounterUpsertRequestDto request, CancellationToken ct);
    Task<ResultDto<bool>> DeleteAsync(EncounterId id, CancellationToken ct);
}
