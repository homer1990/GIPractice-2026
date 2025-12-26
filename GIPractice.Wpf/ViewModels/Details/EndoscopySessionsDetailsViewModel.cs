namespace GIPractice.Wpf.ViewModels;

public sealed class EndoscopySessionsDetailsViewModel : ViewModelBase
{
    private int? _id;
    public int? Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private object? _model;
    public object? Model
    {
        get => _model;
        set => SetProperty(ref _model, value);
    }

    public RelayCommand SaveCommand { get; }
    public RelayCommand DeleteCommand { get; }
    public RelayCommand CloseCommand { get; }

    public EndoscopySessionsDetailsViewModel()
    {
        SaveCommand = new RelayCommand(Save);
        DeleteCommand = new RelayCommand(Delete);
        CloseCommand = new RelayCommand(Close);
    }

    public void Load(int id)
    {
        Id = id;
        // TODO: call EndoscopySessionsModule.GetByIdAsync(...)
    }

    private void Save()
    {
        // TODO: create/update via API
    }

    private void Delete()
    {
        // TODO: delete via API
    }

    private void Close()
    {
        // TODO: navigation close
    }
}
