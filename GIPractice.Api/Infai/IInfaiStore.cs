using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Infai;

namespace GIPractice.Api.Infai;

public interface IInfaiStore
{
    Task<PagedResultDto<InfaiReportListItemDto>> SearchAsync(InfaiReportSearchRequestDto request, CancellationToken ct);
    Task<InfaiReportDto?> GetAsync(InfaiReportId id, CancellationToken ct);
    Task<ResultDto<InfaiReportId>> CreateAsync(InfaiReportUpsertRequestDto request, CancellationToken ct);
    Task<ResultDto<bool>> UpdateAsync(InfaiReportUpsertRequestDto request, CancellationToken ct);
    Task<ResultDto<bool>> DeleteAsync(InfaiReportId id, CancellationToken ct);
}
