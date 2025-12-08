using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using GIPractice.Api.Models;

namespace GIPractice.Client;

public class HomeViewModel : INotifyPropertyChanged
{
    private readonly IPatientPickerService _patientPickerService;

    private string? _statusMessage;
    private PatientListItemDto? _selectedPatient;

    public event PropertyChangedEventHandler? PropertyChanged;

    public HomeViewModel(IPatientPickerService patientPickerService)
    {
        _patientPickerService = patientPickerService;

        PickPatientCommand = new RelayCommand(async _ => await PickPatientAsync());
    }

    public string Title => "Home";

    public string WelcomeText => "Welcome to GIPractice";

    public string? StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (_statusMessage == value) return;
            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    public PatientListItemDto? SelectedPatient
    {
        get => _selectedPatient;
        private set
        {
            if (_selectedPatient == value) return;
            _selectedPatient = value;
            OnPropertyChanged();
        }
    }

    public ICommand PickPatientCommand { get; }

    private async Task PickPatientAsync()
    {
        var patient = await _patientPickerService.PickPatientAsync();

        SelectedPatient = patient;

        if (patient is null)
            StatusMessage = "No patient selected.";
        else
            StatusMessage = $"Selected: {patient.LastName} {patient.FirstName} (#{patient.Id})";
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
