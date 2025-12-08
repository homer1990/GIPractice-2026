using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GIPractice.Client;

public abstract class SingleViewModelBase<TEntity> : ViewModelBase
    where TEntity : class
{
    private TEntity? _entity;
    private bool _isBusy;
    private string? _statusMessage;

    protected readonly AsyncRelayCommand _refreshCommand;
    protected readonly AsyncRelayCommand _saveCommand;

    protected SingleViewModelBase()
    {
        _refreshCommand = new AsyncRelayCommand(_ => ReloadAsync(), _ => Entity != null && !IsBusy);
        _saveCommand = new AsyncRelayCommand(_ => SaveAsync(), _ => Entity != null && CanSave && !IsBusy);
    }

    public TEntity? Entity
    {
        get => _entity;
        private set
        {
            if (SetProperty(ref _entity, value))
            {
                OnEntityChanged(value);
                _saveCommand.RaiseCanExecuteChanged();
                _refreshCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        protected set
        {
            if (SetProperty(ref _isBusy, value))
            {
                _saveCommand.RaiseCanExecuteChanged();
                _refreshCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string? StatusMessage
    {
        get => _statusMessage;
        protected set => SetProperty(ref _statusMessage, value);
    }

    public ICommand RefreshCommand => _refreshCommand;

    public ICommand SaveCommand => _saveCommand;

    protected virtual bool CanSave => true;

    protected virtual void OnEntityChanged(TEntity? entity)
    {
    }

    public async Task LoadAsync(TEntity entity)
    {
        Entity = entity;
        await ReloadAsync();
    }

    public async Task ReloadAsync()
    {
        if (Entity is null) return;

        IsBusy = true;
        StatusMessage = "Loading...";

        try
        {
            await LoadEntityAsync(Entity);
            StatusMessage = "Ready";
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

    public async Task SaveAsync()
    {
        if (Entity is null || !CanSave) return;

        IsBusy = true;
        StatusMessage = "Saving...";

        try
        {
            await SaveEntityAsync(Entity);
            StatusMessage = "Saved.";
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

    protected abstract Task LoadEntityAsync(TEntity entity);

    protected virtual Task SaveEntityAsync(TEntity entity) => Task.CompletedTask;
}
