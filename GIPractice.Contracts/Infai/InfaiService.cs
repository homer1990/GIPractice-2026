using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;

namespace GIPractice.Contracts.Infai;

public interface IInfaiService
{
    Task<ResultDto<PagedResultDto<InfaiReportListItemDto>>> SearchAsync(
        InfaiReportSearchRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<InfaiReportDto>> GetAsync(
        InfaiReportId id,
        CancellationToken cancellationToken = default);

    Task<ResultDto<InfaiReportId>> CreateAsync(
        InfaiReportUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> UpdateAsync(
        InfaiReportUpsertRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> DeleteAsync(
        InfaiReportId id,
        CancellationToken cancellationToken = default);
}
