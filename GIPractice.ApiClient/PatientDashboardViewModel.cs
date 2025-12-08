using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GIPractice.Api.Models;

namespace GIPractice.Client;

public class PatientDashboardViewModel : SingleViewModelBase<PatientListItemDto>
{
    private readonly IPatientsModule _patients;

    private PatientSummaryDto? _summary;

    public PatientDashboardViewModel(IPatientsModule patients)
    {
        _patients = patients;
    }

    public PatientSummaryDto? Summary
    {
        get => _summary;
        private set => SetProperty(ref _summary, value);
    }

    public ObservableCollection<PatientTimelineItemDto> Timeline { get; } = [];

    public string Title =>
        Entity == null
            ? "Patient dashboard"
            : $"{Entity.LastName} {Entity.FirstName} – Dashboard";

    protected override async Task LoadEntityAsync(PatientListItemDto patient)
    {
        StatusMessage = "Loading dashboard...";

        var result = await _patients.GetDashboardAsync(patient.Id);

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

    protected override void OnEntityChanged(PatientListItemDto? entity)
    {
        base.OnEntityChanged(entity);
        OnPropertyChanged(nameof(Title));
    }
}
