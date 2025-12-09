using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GIPractice.Wpf.Backend;

namespace GIPractice.Wpf.ViewModels;

/// <summary>
/// Base for "single entity" screens (details / edit forms).
/// </summary>
public abstract class SingleViewModel<TDto> : ScreenViewModelBase
{
    private TDto? _item;
    private bool _isNew;

    protected SingleViewModel(IDatabase database)
        : base(database)
    {
        LoadCommand = new AsyncRelayCommand(LoadAsync);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
    }

    public TDto? Item
    {
        get => _item;
        protected set => SetProperty(ref _item, value);
    }

    /// <summary>
    /// True if this is a new entity (no ID yet).
    /// </summary>
    public bool IsNew
    {
        get => _isNew;
        protected set => SetProperty(ref _isNew, value);
    }

    public ICommand LoadCommand { get; }
    public ICommand SaveCommand { get; }

    /// <summary>
    /// Provide the query that loads the entity for the current context.
    /// For example: by Id, or new default DTO.
    /// </summary>
    protected abstract IBackendQuery<TDto> CreateLoadQuery();

    /// <summary>
    /// Provide the query that saves the entity (insert/update).
    /// </summary>
    protected abstract IBackendQuery<TDto> CreateSaveQuery(TDto item);

    protected virtual void OnLoaded(TDto item)
    {
        // Hook for derived classes (e.g. compute state from item).
    }

    private Task LoadAsync(CancellationToken cancellationToken)
        => RunBusyAsync(
            async ct =>
            {
                var query = CreateLoadQuery();
                var dto = await Database.QueryAsync(query, ct).ConfigureAwait(false);

                Item = dto;
                IsNew = DetermineIsNew(dto);
                OnLoaded(dto);
            },
            busyText: "Loading…",
            externalToken: cancellationToken);

    private Task SaveAsync(CancellationToken cancellationToken)
        => RunBusyAsync(
            async ct =>
            {
                if (Item is null)
                    return;

                var query = CreateSaveQuery(Item);
                Item = await Database.QueryAsync(query, ct).ConfigureAwait(false);
                IsNew = DetermineIsNew(Item);
            },
            busyText: "Saving…",
            externalToken: cancellationToken);

    /// <summary>
    /// How do we know if the DTO is "new"? By default always false;
    /// concrete VM can override and inspect an Id property.
    /// </summary>
    protected virtual bool DetermineIsNew(TDto item) => false;
}
