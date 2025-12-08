using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GIPractice.ApiClient;
using GIPractice.Client.Core;

namespace GIPractice.Wpf;

public class DashboardViewModel : PresentationBase
{
    private readonly IDatabaseController _database;
    private readonly LocalizationManager _localization;
    private string _status;

    public DashboardViewModel(IDatabaseController database, LocalizationManager localization)
    {
        _database = database;
        _localization = localization;
        _status = _localization["Status.Connecting"];

        _database.StatusChanged += (_, change) =>
        {
            Status = change.Message ?? change.Status.ToString();
        };

        RefreshCommand = new DelegateCommand(RecordInteractionAsync);
    }

    public string Title => "Workspace";

    public ObservableCollection<string> ActivityLog { get; } = new();

    public string Status
    {
        get => _status;
        private set => SetField(ref _status, value);
    }

    public DelegateCommand RefreshCommand { get; }

    private Task RecordInteractionAsync()
    {
        _database.RecordActivity();
        ActivityLog.Add($"{DateTime.Now:t} {_localization["Status.Connected"]}");
        return Task.CompletedTask;
    }
}
