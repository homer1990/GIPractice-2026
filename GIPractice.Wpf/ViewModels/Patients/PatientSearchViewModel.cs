using System;
using System.Threading;
using System.Threading.Tasks;
using GIPractice.Api.Models;
using GIPractice.Wpf.Backend;
using GIPractice.Wpf.Backend.Queries;
using GIPractice.Wpf.ViewModels.Search;

namespace GIPractice.Wpf.ViewModels.Patients;

/// <summary>
/// Concrete patients search screen.
/// </summary>
public sealed class PatientSearchViewModel
    : PagedSearchViewModel<PatientSearchRequestDto, PagedResultDto<PatientListItemDto>, PatientListItemDto>
{
    public PatientSearchViewModel(IDatabase database)
        : base(database, new PatientSearchRequestDto())
    {
    }

    protected override IBackendQuery<PagedResultDto<PatientListItemDto>> CreateSearchQuery(PatientSearchRequestDto criteria)
        => new SearchPatientsQuery(criteria);

    protected override ReadOnlyMemory<PatientListItemDto> GetItemsFromResult(PagedResultDto<PatientListItemDto> result)
    {
        // ASSUMPTION: PagedResultDto has an Items collection.
        // If the property name differs, change it here.
        var items = result.Items ?? [];
        return new ReadOnlyMemory<PatientListItemDto>(items.ToArray());
    }

    protected override int GetTotalCount(PagedResultDto<PatientListItemDto> result)
    {
        // ASSUMPTION: PagedResultDto has a TotalCount property.
        // If your DTO calls it something else, adjust this line.
        return result.TotalCount;
    }

    protected override void ApplyPagingToCriteria(ref PatientSearchRequestDto criteria, int pageIndex, int pageSize)
    {
        // ASSUMPTION: PatientSearchRequestDto has PageIndex/PageSize.
        // If it uses different names (e.g. Page/Take), modify accordingly.

        criteria.PageIndex = pageIndex;
        criteria.PageSize = pageSize;
    }

    // (Optional) Convenience method to trigger initial search on screen open.
    public Task LoadAsync(CancellationToken cancellationToken = default)
        => SearchCommand is AsyncRelayCommand cmd
            ? cmd.ExecuteAsync(cancellationToken)
            : Task.CompletedTask;
}
