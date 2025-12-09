using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GIPractice.Wpf.Backend;

namespace GIPractice.Wpf.ViewModels.Search;

/// <summary>
/// Generic base for paged search screens.
/// TSearch : DTO with search filters + page index/size.
/// TResult : result DTO returned by backend (e.g. PagedResultDto<TItem>).
/// TItem   : DTO displayed in the grid/list.
/// </summary>
public abstract class PagedSearchViewModel<TSearch, TResult, TItem> : ScreenViewModelBase
{
    private TSearch _searchCriteria;
    private TItem? _selectedItem;
    private int _pageIndex;
    private int _pageSize = 20;
    private int _totalCount;

    protected PagedSearchViewModel(IDatabase database, TSearch initialCriteria)
        : base(database)
    {
        _searchCriteria = initialCriteria;

        Items = [];

        SearchCommand = new AsyncRelayCommand(SearchAsync);
        NextPageCommand = new AsyncRelayCommand(NextPageAsync, () => CanChangePage(1));
        PreviousPageCommand = new AsyncRelayCommand(PreviousPageAsync, () => CanChangePage(-1));
    }

    public ObservableCollection<TItem> Items { get; }

    public TItem? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }

    /// <summary>
    /// Current page index (0-based).
    /// </summary>
    public int PageIndex
    {
        get => _pageIndex;
        set
        {
            if (SetProperty(ref _pageIndex, value))
                RaisePagingCanExecuteChanged();
        }
    }

    /// <summary>
    /// Page size (items per page).
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (SetProperty(ref _pageSize, value))
                RaisePagingCanExecuteChanged();
        }
    }

    /// <summary>
    /// Total items reported by backend.
    /// </summary>
    public int TotalCount
    {
        get => _totalCount;
        protected set
        {
            if (SetProperty(ref _totalCount, value))
                RaisePagingCanExecuteChanged();
        }
    }

    public TSearch SearchCriteria
    {
        get => _searchCriteria;
        set => SetProperty(ref _searchCriteria, value);
    }

    public ICommand SearchCommand { get; }
    public AsyncRelayCommand NextPageCommand { get; }
    public AsyncRelayCommand PreviousPageCommand { get; }

    protected abstract IBackendQuery<TResult> CreateSearchQuery(TSearch criteria);

    /// <summary>
    /// Extract the collection of items from backend result.
    /// </summary>
    protected abstract ReadOnlyMemory<TItem> GetItemsFromResult(TResult result);

    /// <summary>
    /// Extract the total count from backend result.
    /// </summary>
    protected abstract int GetTotalCount(TResult result);

    /// <summary>
    /// Override if your TSearch stores page info differently.
    /// </summary>
    protected virtual void ApplyPagingToCriteria(ref TSearch criteria, int pageIndex, int pageSize)
    {
        // Default: do nothing. Concrete VM can override and set criteria.PageIndex/PageSize.
        // We use "ref" so you can mutate a struct DTO if you ever choose to.
    }

    private async Task SearchAsync(CancellationToken cancellationToken)
    {
        await RunBusyAsync(
            async ct =>
            {
                var criteria = SearchCriteria;
                ApplyPagingToCriteria(ref criteria, PageIndex, PageSize);

                var query = CreateSearchQuery(criteria);
                var result = await Database.QueryAsync(query, ct).ConfigureAwait(false);

                var items = GetItemsFromResult(result);
                TotalCount = GetTotalCount(result);

                Items.Clear();
                foreach (var item in items.ToArray())
                    Items.Add(item);
            },
            busyText: "Searching…",
            externalToken: cancellationToken);
    }

    private bool CanChangePage(int delta)
    {
        if (PageSize <= 0) return false;
        if (TotalCount <= 0) return false;

        var maxPageIndex = (TotalCount - 1) / PageSize;
        var newIndex = PageIndex + delta;
        return newIndex >= 0 && newIndex <= maxPageIndex;
    }

    private Task NextPageAsync(CancellationToken token)
    {
        if (!CanChangePage(1))
            return Task.CompletedTask;

        PageIndex++;
        return SearchAsync(token);
    }

    private Task PreviousPageAsync(CancellationToken token)
    {
        if (!CanChangePage(-1))
            return Task.CompletedTask;

        PageIndex--;
        return SearchAsync(token);
    }

    private void RaisePagingCanExecuteChanged()
    {
        NextPageCommand.RaiseCanExecuteChanged();
        PreviousPageCommand.RaiseCanExecuteChanged();
    }
}
