using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GIPractice.Api.Models;

namespace GIPractice.Client;

public class PatientDashboardViewModel(IPatientsModule patients) : INotifyPropertyChanged
{
    private readonly IPatientsModule _patients = patients;

    private PatientListItemDto? _patient;
    private PatientSummaryDto? _summary;
    private bool _isBusy;
    private string? _statusMessage;

    public event PropertyChangedEventHandler? PropertyChanged;

    public PatientListItemDto? Patient
    {
        get => _patient;
        private set { _patient = value; OnPropertyChanged(); OnPropertyChanged(nameof(Title)); }
    }

    public PatientSummaryDto? Summary
    {
        get => _summary;
        private set { _summary = value; OnPropertyChanged(); }
    }

    public ObservableCollection<PatientTimelineItemDto> Timeline { get; } = [];

    public bool IsBusy
    {
        get => _isBusy;
        private set { _isBusy = value; OnPropertyChanged(); }
    }

    public string? StatusMessage
    {
        get => _statusMessage;
        private set { _statusMessage = value; OnPropertyChanged(); }
    }

    public string Title =>
        Patient == null
            ? "Patient dashboard"
            : $"{Patient.LastName} {Patient.FirstName} – Dashboard";

    public async Task InitializeAsync(PatientListItemDto patient)
    {
        Patient = patient;
        await LoadAsync();
    }

    public async Task LoadAsync()
    {
        if (Patient == null) return;

        IsBusy = true;
        StatusMessage = "Loading dashboard...";

        try
        {
            var result = await _patients.GetDashboardAsync(Patient.Id);

            if (result == null)
            {
                StatusMessage = "No dashboard data.";
                return;
            }

            Summary = result.Summary;

            Timeline.Clear();
            if (result.Timeline != null)
            {
                foreach (var item in result.Timeline)
                    Timeline.Add(item);
            }

            StatusMessage = $"Loaded {Timeline.Count} timeline item(s).";
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
    public Task LoadAsync(PatientListItemDto patient)
    {
        Patient = patient;
        // Reuse the existing logic
        return LoadAsync();
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
