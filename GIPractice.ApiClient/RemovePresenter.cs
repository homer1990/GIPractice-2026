using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using GIPractice.Client.Core;

namespace GIPractice.ApiClient;

public abstract class RemovePresenter<TEntityDto, TQuery> : PresentationBase
{
    private readonly IDatabaseController _database;

    protected RemovePresenter(IDatabaseController database)
    {
        _database = database;
    }

    public ObservableCollection<TEntityDto> Selected { get; } = new();

    protected abstract string RemoveRoute { get; }
    protected abstract TQuery BuildRemoveQuery(IReadOnlyCollection<TEntityDto> entities);

    public async Task RemoveAsync(CancellationToken cancellationToken)
    {
        if (Selected.Count == 0)
        {
            return;
        }

        var payload = BuildRemoveQuery(Selected);
        await _database.QueryAsync<TQuery, TEntityDto>(RemoveRoute, payload, cancellationToken).ConfigureAwait(false);
        Selected.Clear();
    }
}
