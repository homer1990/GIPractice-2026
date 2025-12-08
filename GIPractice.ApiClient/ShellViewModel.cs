using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace GIPractice.Client;

public class ShellViewModel : INotifyPropertyChanged
{
    private readonly ClientController _controller;
    private object? _currentContent;
    private string? _bannerMessage;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ShellViewModel(ClientController controller, HomeViewModel home, PatientSearchViewModel patients)
    {
        _controller = controller;
        Home = home;
        Patients = patients;

        // Start on Home
        _currentContent = Home;

        NavigateHomeCommand = new RelayCommand(_ => CurrentContent = Home);
        NavigatePatientsCommand = new RelayCommand(_ => CurrentContent = Patients);

        _controller.ConnectivityLost += (_, _) => BannerMessage = "Connection lost. Trying to reconnect...";
        _controller.ConnectivityRestored += (_, _) => BannerMessage = null;
        _controller.TokenExpiring += (_, _) => BannerMessage = "Session will expire soon. Refreshing...";
        _controller.TokenRefreshed += (_, _) => BannerMessage = null;
    }

    public HomeViewModel Home { get; }

    public PatientSearchViewModel Patients { get; }

    public object? CurrentContent
    {
        get => _currentContent;
        private set
        {
            if (Equals(_currentContent, value)) return;
            _currentContent = value;
            OnPropertyChanged();
        }
    }

    public string? BannerMessage
    {
        get => _bannerMessage;
        private set
        {
            if (_bannerMessage == value) return;
            _bannerMessage = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasBanner));
        }
    }

    public bool HasBanner => !string.IsNullOrWhiteSpace(BannerMessage);

    public ICommand NavigateHomeCommand { get; }

    public ICommand NavigatePatientsCommand { get; }

    public ICommand OpenSettingsCommand { get; }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
