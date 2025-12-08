using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GIPractice.Client.Core;

namespace GIPractice.ApiClient;

public abstract class SinglePresenter<TEntityDto, TQuery> : PresentationBase
{
    private readonly IDatabaseController _database;
    private TEntityDto? _entity;
    private bool _isNew;
    private bool _isDirty;

    protected SinglePresenter(IDatabaseController database)
    {
        _database = database;
    }

    public TEntityDto? Entity
    {
        get => _entity;
        protected set
        {
            SetField(ref _entity, value);
            IsDirty = false;
        }
    }

    public bool IsNew
    {
        get => _isNew;
        protected set => SetField(ref _isNew, value);
    }

    public bool IsDirty
    {
        get => _isDirty;
        protected set => SetField(ref _isDirty, value);
    }

    protected abstract string LoadRoute { get; }
    protected abstract string SaveRoute { get; }

    protected abstract TQuery BuildSaveQuery(TEntityDto entity);

    public virtual async Task LoadAsync(TQuery query, CancellationToken cancellationToken)
    {
        var results = await _database.QueryAsync<TQuery, TEntityDto>(LoadRoute, query, cancellationToken).ConfigureAwait(false);
        Entity = results.FirstOrDefault();
        IsNew = Entity is null;
    }

    public virtual async Task SaveAsync(CancellationToken cancellationToken)
    {
        if (Entity is null)
        {
            return;
        }

        var payload = BuildSaveQuery(Entity);
        await _database.QueryAsync<TQuery, TEntityDto>(SaveRoute, payload, cancellationToken).ConfigureAwait(false);
        IsDirty = false;
        IsNew = false;
    }
}
