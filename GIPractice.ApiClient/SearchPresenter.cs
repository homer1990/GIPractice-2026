using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using GIPractice.Client.Core;

namespace GIPractice.ApiClient;

public abstract class SearchPresenter<TSearchDto, TQuery> : PresentationBase
{
    private readonly IDatabaseController _database;
    private bool _isBusy;
    private string? _status;

    protected SearchPresenter(IDatabaseController database)
    {
        _database = database;
    }

    public ObservableCollection<TSearchDto> Results { get; } = new();

    public bool IsBusy
    {
        get => _isBusy;
        private set => SetField(ref _isBusy, value);
    }

    public string? Status
    {
        get => _status;
        private set => SetField(ref _status, value);
    }

    protected abstract Task<TQuery> BuildQueryAsync(CancellationToken cancellationToken);

    protected abstract string Route { get; }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        Status = "Loading";

        var query = await BuildQueryAsync(cancellationToken).ConfigureAwait(false);
        var items = await _database.QueryAsync<TQuery, TSearchDto>(Route, query, cancellationToken).ConfigureAwait(false);

        Results.Clear();
        foreach (var item in items)
        {
            Results.Add(item);
        }

        Status = $"{Results.Count} results";
        IsBusy = false;
    }
}
