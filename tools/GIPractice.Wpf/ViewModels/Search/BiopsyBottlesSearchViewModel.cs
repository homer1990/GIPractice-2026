using System.Collections.ObjectModel;

namespace GIPractice.Wpf.ViewModels;

public sealed class BiopsyBottlesSearchViewModel : ViewModelBase
{
    private string? _query;
    public string? Query
    {
        get => _query;
        set => SetProperty(ref _query, value);
    }

    public ObservableCollection<object> Items { get; } = new();

    public RelayCommand SearchCommand { get; }
    public RelayCommand NewCommand { get; }
    public RelayCommand<object> OpenCommand { get; }

    public BiopsyBottlesSearchViewModel()
    {
        SearchCommand = new RelayCommand(Search);
        NewCommand = new RelayCommand(New);
        OpenCommand = new RelayCommand<object>(Open);
    }

    private void Search()
    {
        // TODO: call BiopsyBottlesModule.SearchAsync(...)
        Items.Clear();
    }

    private void New()
    {
        // TODO: navigate/open details editor in create mode
    }

    private void Open(object item)
    {
        // TODO: item -> id -> load details
    }
}
