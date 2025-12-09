using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GIPractice.Wpf.Backend;

namespace GIPractice.Wpf.ViewModels;

/// <summary>
/// Base for "remove / archive" screens, working over a set of DTOs.
/// </summary>
public abstract class RemoveViewModel<TDto> : ScreenViewModelBase
{
    private TDto? _selectedItem;

    protected RemoveViewModel(IDatabase database)
        : base(database)
    {
        RemoveCommand = new AsyncRelayCommand(RemoveAsync, () => SelectedItem is not null);
    }

    public ObservableCollection<TDto> Items { get; } = [];

    public TDto? SelectedItem
    {
        get => _selectedItem;
        set
        {
            if (SetProperty(ref _selectedItem, value))
                ((AsyncRelayCommand)RemoveCommand).RaiseCanExecuteChanged();
        }
    }

    public ICommand RemoveCommand { get; }

    /// <summary>
    /// Provide the query that will remove the selected DTO.
    /// </summary>
    protected abstract IBackendQuery<bool> CreateRemoveQuery(TDto item);

    /// <summary>
    /// Override if you want a custom remove behaviour before/after calling the backend.
    /// </summary>
    protected virtual void OnRemoved(TDto item)
    {
        Items.Remove(item);
    }

    private Task RemoveAsync(CancellationToken cancellationToken)
        => RunBusyAsync(
            async ct =>
            {
                if (SelectedItem is null)
                    return;

                var item = SelectedItem;
                var query = CreateRemoveQuery(item);
                var success = await Database.QueryAsync(query, ct).ConfigureAwait(false);

                if (success)
                    OnRemoved(item);
            },
            busyText: "Removing…",
            externalToken: cancellationToken);
}
