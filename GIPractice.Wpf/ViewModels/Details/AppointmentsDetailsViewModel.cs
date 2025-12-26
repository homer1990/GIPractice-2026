namespace GIPractice.Wpf.ViewModels;

public sealed class AppointmentsDetailsViewModel : ViewModelBase
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

    public AppointmentsDetailsViewModel()
    {
        SaveCommand = new RelayCommand(Save);
        DeleteCommand = new RelayCommand(Delete);
        CloseCommand = new RelayCommand(Close);
    }

    public void Load(int id)
    {
        Id = id;
        // TODO: call AppointmentsModule.GetByIdAsync(...)
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
