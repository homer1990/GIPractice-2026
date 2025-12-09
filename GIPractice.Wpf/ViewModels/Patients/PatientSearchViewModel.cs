using GIPractice.Api.Models;
using GIPractice.Wpf.Backend;
using GIPractice.Wpf.Backend.Queries;
using GIPractice.Wpf.ViewModels.Search;

namespace GIPractice.Wpf.ViewModels.Patients;

public sealed class PatientSearchViewModel
    : PagedSearchViewModel<PatientSearchRequestDto, PagedResultDto<PatientSummaryDto>, PatientSummaryDto>
{
    public PatientSearchViewModel(IDatabase database)
        : base(database, new PatientSearchRequestDto())
    {
    }

    protected override IBackendQuery<PagedResultDto<PatientSummaryDto>> CreateSearchQuery(PatientSearchRequestDto criteria)
        => new SearchPatientsQuery(criteria);

    protected override ReadOnlyMemory<PatientSummaryDto> GetItemsFromResult(PagedResultDto<PatientSummaryDto> result)
    {
        // Adjust property name if your PagedResultDto uses a different one
        var items = result.Items ?? [];
        return new ReadOnlyMemory<PatientSummaryDto>(items.ToArray());
    }

    protected override int GetTotalCount(PagedResultDto<PatientSummaryDto> result)
    {
        // Adjust if your property isn’t called TotalCount
        return result.TotalCount;
    }

    protected override void ApplyPagingToCriteria(ref PatientSearchRequestDto criteria, int pageIndex, int pageSize)
    {
        // Adjust if your search request uses different property names
        criteria.PageIndex = pageIndex;
        criteria.PageSize = pageSize;
    }
}
