using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using GIPractice.Api.Models;

namespace GIPractice.Client;

public interface ISearchViewModel
{
    Task ApplySortAsync(string sortField, bool descending);
}

public abstract class SearchViewModelBase<TItem, TRequest> : ViewModelBase, ISearchViewModel
    where TItem : class
    where TRequest : class
{
    private readonly INavigationService? _navigation;

    private int _page = 1;
    private int _pageSize = 50;
    private int _totalCount;
    private bool _isBusy;
    private string? _statusMessage;
    private string? _sortField;
    private bool _sortDescending = true;
    private TItem? _selectedItem;

    protected readonly AsyncRelayCommand _searchCommand;
    protected readonly AsyncRelayCommand _nextPageCommand;
    protected readonly AsyncRelayCommand _previousPageCommand;
    protected readonly AsyncRelayCommand _openDetailsCommand;

    protected SearchViewModelBase(INavigationService? navigation = null)
    {
        _navigation = navigation;
        Items = new ObservableCollection<TItem>();

        _searchCommand = new AsyncRelayCommand(
            _ => SearchAsync(resetPage: true),
            _ => !IsBusy && Validate(out _));

        _nextPageCommand = new AsyncRelayCommand(
            _ => NextPageAsync(),
            _ => !IsBusy && HasNextPage);

        _previousPageCommand = new AsyncRelayCommand(
            _ => PreviousPageAsync(),
            _ => !IsBusy && HasPreviousPage);

        _openDetailsCommand = new AsyncRelayCommand(_ => ActivateSelectedItemAsync(), _ => SelectedItem != null);
    }

    public ObservableCollection<TItem> Items { get; }

    public Func<TItem, Task>? SelectionHandler { get; set; }

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

    public string PageInfo => $"Page {Page} / {TotalPages}";

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

    public TItem? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value))
                _openDetailsCommand.RaiseCanExecuteChanged();
        }
    }

    public ICommand SearchCommand => _searchCommand;
    public ICommand NextPageCommand => _nextPageCommand;
    public ICommand PreviousPageCommand => _previousPageCommand;
    public ICommand OpenDetailsCommand => _openDetailsCommand;

    protected void RaisePagingCanExecuteChanged()
    {
        _searchCommand.RaiseCanExecuteChanged();
        _nextPageCommand.RaiseCanExecuteChanged();
        _previousPageCommand.RaiseCanExecuteChanged();
    }

    protected abstract TRequest BuildRequestCore();

    protected abstract Task<PagedResultDto<TItem>> ExecuteSearchAsync(TRequest request);

    protected virtual bool Validate(out string? error)
    {
        error = null;
        return true;
    }

    protected virtual Task OnItemActivatedAsync(TItem item)
    {
        if (SelectionHandler != null)
            return SelectionHandler.Invoke(item);

        if (_navigation != null)
            return NavigateAsync(item);

        return Task.CompletedTask;
    }

    protected virtual Task NavigateAsync(TItem item) => Task.CompletedTask;

    public async Task SearchAsync(bool resetPage)
    {
        if (IsBusy) return;

        if (!Validate(out var validationError))
        {
            StatusMessage = validationError;
            return;
        }

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

    public async Task ApplySortAsync(string sortField, bool descending)
    {
        SortField = sortField;
        SortDescending = descending;
        Page = 1;
        await SearchAsync(resetPage: false);
    }

    private Task ActivateSelectedItemAsync()
    {
        if (SelectedItem is null)
            return Task.CompletedTask;

        return OnItemActivatedAsync(SelectedItem);
    }
}
