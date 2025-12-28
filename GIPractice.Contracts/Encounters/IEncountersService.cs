using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Encounters;

public interface IEncountersService
{
    Task<ResultDto<PagedResultDto<EncounterListItemDto>>> SearchAsync(
        EncounterSearchRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<EncounterDetailsDto>> GetAsync(
        EncounterId id,
        CancellationToken cancellationToken = default);

    Task<ResultDto<EncounterId>> CreateAsync(
        EncounterUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> UpdateAsync(
        EncounterUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> DeleteAsync(
        EncounterId id,
        CancellationToken cancellationToken = default);
}
