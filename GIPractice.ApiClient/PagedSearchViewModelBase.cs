// PagedSearchViewModelBase.cs
using GIPractice.Api.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GIPractice.Client;

public abstract class PagedSearchViewModelBase<TItem, TRequest> : ViewModelBase
    where TItem : class
    where TRequest : class
{
    private int _page = 1;
    private int _pageSize = 50;
    private int _totalCount;
    private bool _isBusy;
    private string? _statusMessage;
    private string? _sortField;
    private bool _sortDescending = true;

    protected readonly AsyncRelayCommand _searchCommand;
    protected readonly AsyncRelayCommand _nextPageCommand;
    protected readonly AsyncRelayCommand _previousPageCommand;

    protected PagedSearchViewModelBase()
    {
        Items = new ObservableCollection<TItem>();

        _searchCommand = new AsyncRelayCommand(
            _ => SearchAsync(resetPage: true),
            _ => !IsBusy);

        _nextPageCommand = new AsyncRelayCommand(
            _ => NextPageAsync(),
            _ => !IsBusy && HasNextPage);

        _previousPageCommand = new AsyncRelayCommand(
            _ => PreviousPageAsync(),
            _ => !IsBusy && HasPreviousPage);
    }

    // The list the DataGrid binds to
    public ObservableCollection<TItem> Items { get; }

    public string? StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        protected set
        {
            if (SetProperty(ref _isBusy, value))
                RaisePagingCanExecuteChanged();
        }
    }

    public int Page
    {
        get => _page;
        set
        {
            if (SetProperty(ref _page, value))
                RaisePagingCanExecuteChanged();
        }
    }

    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (SetProperty(ref _pageSize, value))
                OnPropertyChanged(nameof(TotalPages));
        }
    }

    public int TotalCount
    {
        get => _totalCount;
        protected set
        {
            if (SetProperty(ref _totalCount, value))
            {
                OnPropertyChanged(nameof(TotalPages));
                RaisePagingCanExecuteChanged();
            }
        }
    }

    public int TotalPages => PageSize <= 0
        ? 1
        : Math.Max(1, (int)Math.Ceiling((double)TotalCount / PageSize));

    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public string? SortField
    {
        get => _sortField;
        set => SetProperty(ref _sortField, value);
    }

    public bool SortDescending
    {
        get => _sortDescending;
        set => SetProperty(ref _sortDescending, value);
    }

    public ICommand SearchCommand => _searchCommand;
    public ICommand NextPageCommand => _nextPageCommand;
    public ICommand PreviousPageCommand => _previousPageCommand;

    protected void RaisePagingCanExecuteChanged()
    {
        _searchCommand.RaiseCanExecuteChanged();
        _nextPageCommand.RaiseCanExecuteChanged();
        _previousPageCommand.RaiseCanExecuteChanged();
    }

    // Called from concrete VM to build the request object with filters
    protected abstract TRequest BuildRequestCore();

    // Called from concrete VM to actually talk to the API
    protected abstract Task<PagedResultDto<TItem>> ExecuteSearchAsync(TRequest request);

    /// <summary>
    /// Public search API used by Search button and header-clicks.
    /// </summary>
    public async Task SearchAsync(bool resetPage)
    {
        if (IsBusy) return;

        if (resetPage)
            Page = 1;

        await RunSearchAsync();
    }

    private async Task RunSearchAsync()
    {
        IsBusy = true;
        StatusMessage = "Searching…";

        try
        {
            var request = BuildRequestCore();

            // It’s fine if TRequest doesn’t *derive* from PagedRequestDto;
            // we just expect these members to exist. If you want, you
            // can constrain TRequest : PagedRequestDto instead.
            dynamic r = request!;
            r.Page = Page;
            r.PageSize = PageSize;
            r.SortField = SortField;
            r.SortDescending = SortDescending;

            var result = await ExecuteSearchAsync(request);

            TotalCount = result.TotalCount;

            Items.Clear();
            foreach (var item in result.Items)
                Items.Add(item);

            StatusMessage = $"Found {TotalCount} item(s). Showing {Items.Count}.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.InnerException?.Message ?? ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task NextPageAsync()
    {
        if (!HasNextPage || IsBusy) return;

        Page++;
        await RunSearchAsync();
    }

    private async Task PreviousPageAsync()
    {
        if (!HasPreviousPage || IsBusy) return;

        Page--;
        await RunSearchAsync();
    }

    /// <summary>
    /// Called from DataGrid header click to apply sorting & requery.
    /// </summary>
    public async Task ApplySortAsync(string sortField, bool descending)
    {
        SortField = sortField;
        SortDescending = descending;
        Page = 1;
        await SearchAsync(resetPage: false);
    }
}
