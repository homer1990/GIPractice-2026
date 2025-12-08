using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace GIPractice.Client;

public class ShellViewModel : INotifyPropertyChanged
{
    private object? _currentContent;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ShellViewModel(HomeViewModel home, PatientSearchViewModel patients)
    {
        Home = home;
        Patients = patients;

        // Start on Home
        _currentContent = Home;

        NavigateHomeCommand = new RelayCommand(_ => CurrentContent = Home);
        NavigatePatientsCommand = new RelayCommand(_ => CurrentContent = Patients);
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

    public ICommand NavigateHomeCommand { get; }

    public ICommand NavigatePatientsCommand { get; }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
