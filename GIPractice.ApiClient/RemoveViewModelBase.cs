using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GIPractice.Client;

public abstract class RemoveViewModelBase<TEntity> : SingleViewModelBase<TEntity>
    where TEntity : class
{
    private readonly AsyncRelayCommand _removeCommand;

    protected RemoveViewModelBase()
    {
        _removeCommand = new AsyncRelayCommand(_ => RemoveAsync(), _ => Entity != null && !IsBusy);
    }

    public ICommand RemoveCommand => _removeCommand;

    protected abstract Task RemoveEntityAsync(TEntity entity);

    public async Task RemoveAsync()
    {
        if (Entity is null) return;

        IsBusy = true;
        StatusMessage = "Removing...";

        try
        {
            await RemoveEntityAsync(Entity);
            StatusMessage = "Removed.";
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
}
