using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GIPractice.Wpf.Backend;

namespace GIPractice.Wpf.ViewModels;

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

    public event EventHandler? Saved;            // <-- NEW

    public TDto? Item
    {
        get => _item;
        protected set => SetProperty(ref _item, value);
    }

    public bool IsNew
    {
        get => _isNew;
        protected set => SetProperty(ref _isNew, value);
    }

    public ICommand LoadCommand { get; }
    public ICommand SaveCommand { get; }

    protected abstract IBackendQuery<TDto> CreateLoadQuery();
    protected abstract IBackendQuery<TDto> CreateSaveQuery(TDto item);

    protected virtual void OnLoaded(TDto item) { }

    protected virtual bool DetermineIsNew(TDto item) => false;

    public Task LoadAsync(CancellationToken cancellationToken = default)
        => RunBusyAsync(
            async ct =>
            {
                var query = CreateLoadQuery();
                var dto = await Database.QueryAsync(query, ct);
                Item = dto;
                IsNew = DetermineIsNew(dto);
                OnLoaded(dto);
            },
            busyText: "Loading…",
            externalToken: cancellationToken);

    public Task SaveAsync(CancellationToken cancellationToken = default)
        => RunBusyAsync(
            async ct =>
            {
                if (Item is null)
                    return;

                var query = CreateSaveQuery(Item);
                Item = await Database.QueryAsync(query, ct);
                IsNew = DetermineIsNew(Item);

                Saved?.Invoke(this, EventArgs.Empty);   // <-- raise event
            },
            busyText: "Saving…",
            externalToken: cancellationToken);
}
