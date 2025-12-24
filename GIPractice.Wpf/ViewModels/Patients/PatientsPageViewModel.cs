using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace GIPractice.Wpf.ViewModels;

public sealed class PatientsPageViewModel : ViewModelBase
{
    private bool _isSchedulerOpen;
    private NewAppointmentDayViewModel? _appointmentScheduler;

    public bool IsSchedulerOpen
    {
        get => _isSchedulerOpen;
        set => SetProperty(ref _isSchedulerOpen, value);
    }

    public NewAppointmentDayViewModel? AppointmentScheduler
    {
        get => _appointmentScheduler;
        set => SetProperty(ref _appointmentScheduler, value);
    }

    private PatientVm? _selectedPatient;
    private bool _isSearchOpen;
    private string? _searchText;
    private PatientVm? _searchSelectedPatient;

    public PatientVm? SelectedPatient
    {
        get => _selectedPatient;
        set
        {
            if (_selectedPatient != null)
                _selectedPatient.PropertyChanged -= SelectedPatient_PropertyChanged;

            if (!SetProperty(ref _selectedPatient, value))
                return;

            if (_selectedPatient != null)
                _selectedPatient.PropertyChanged += SelectedPatient_PropertyChanged;

            LoadDummyDashboardForSelectedPatient();
            RaiseAllCanExecutes();
        }
    }

    public bool IsSearchOpen
    {
        get => _isSearchOpen;
        set => SetProperty(ref _isSearchOpen, value);
    }

    public string? SearchText
    {
        get => _searchText;
        set
        {
            if (!SetProperty(ref _searchText, value))
                return;
            PerformSearch(); // live filter
        }
    }

    public ObservableCollection<PatientVm> AllPatients { get; } = new();
    public ObservableCollection<PatientVm> SearchResults { get; } = new();

    public PatientVm? SearchSelectedPatient
    {
        get => _searchSelectedPatient;
        set
        {
            if (!SetProperty(ref _searchSelectedPatient, value))
                return;
            SelectSearchPatientCommand.RaiseCanExecuteChanged();
        }
    }

    public ObservableCollection<string> GenderOptions { get; } =
        new() { "Άρρεν", "Θήλυ", "Άλλο..." };

    // Dummy dashboard collections (you added these VMs in PatientDashboardRowVms.cs)
    public ObservableCollection<PendingBiopsyVm> PendingBiopsies { get; } = new();
    public ObservableCollection<PendingPathologyVm> PendingPathologyReports { get; } = new();
    public ObservableCollection<PendingAppointmentVm> PendingAppointments { get; } = new();
    public ObservableCollection<VisitVm> PatientVisits { get; } = new();

    // Continuity: reuse AppointmentVm from scheduler work
    public ObservableCollection<AppointmentVm> PatientAppointments { get; } = new();

    public ObservableCollection<EndoscopyVm> PatientEndoscopies { get; } = new();
    public ObservableCollection<PathologyReportVm> PatientPathologyReports { get; } = new();

    // Commands
    public RelayCommand ToggleSearchCommand { get; }
    public RelayCommand PerformSearchCommand { get; }
    public RelayCommand SelectSearchPatientCommand { get; }

    public RelayCommand NewPatientCommand { get; }
    public RelayCommand SavePatientCommand { get; }
    public RelayCommand DeletePatientCommand { get; }

    public RelayCommand CapturePhotoCommand { get; }
    public RelayCommand PhotoFromFileCommand { get; }
    public RelayCommand RefreshPatientCommand { get; } // ΕΠΑΝΑΦΟΡΑ: reset photo baseline

    // fields
    private readonly RelayCommand _newAppointmentCommand;
    private readonly RelayCommand _closeSchedulerCommand;

    // properties
    public RelayCommand NewAppointmentCommand => _newAppointmentCommand;
    public RelayCommand CloseSchedulerCommand => _closeSchedulerCommand;

    public PatientsPageViewModel()
    {
        ToggleSearchCommand = new RelayCommand(() => IsSearchOpen = !IsSearchOpen);
        PerformSearchCommand = new RelayCommand(PerformSearch);
        SelectSearchPatientCommand = new RelayCommand(SelectSearchPatient, () => SearchSelectedPatient != null);

        NewPatientCommand = new RelayCommand(NewPatient);
        SavePatientCommand = new RelayCommand(SavePatient, () => SelectedPatient != null);
        DeletePatientCommand = new RelayCommand(DeletePatient, () => SelectedPatient != null);

        CapturePhotoCommand = new RelayCommand(CapturePhoto, () => SelectedPatient != null);
        PhotoFromFileCommand = new RelayCommand(PhotoFromFile, () => SelectedPatient != null);

        RefreshPatientCommand = new RelayCommand(
            () => SelectedPatient?.ResetPhotoToBaseline(),
            () => SelectedPatient != null && SelectedPatient.IsPhotoDirty);

        _newAppointmentCommand = new RelayCommand(OpenScheduler, () => SelectedPatient != null);
        _closeSchedulerCommand = new RelayCommand(() => IsSchedulerOpen = false);

        SeedDummyPatients();
        PerformSearch();
        SelectedPatient = AllPatients.FirstOrDefault();
    }

    private void SelectedPatient_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PatientVm.IsPhotoDirty) ||
            e.PropertyName == nameof(PatientVm.PhotoImageSource))
        {
            RefreshPatientCommand.RaiseCanExecuteChanged();
        }
    }

    private void SeedDummyPatients()
    {
        AllPatients.Clear();

        var p1 = PatientVm.CreateNew();
        p1.Id = 1; p1.FirstName = "Γιώργος"; p1.LastName = "Παπαδόπουλος"; p1.PhoneNumber = "69xxxxxxxx";
        p1.AcceptPhotoAsBaseline();

        var p2 = PatientVm.CreateNew();
        p2.Id = 2; p2.FirstName = "Μαρία"; p2.LastName = "Ιωάννου"; p2.PhoneNumber = "69yyyyyyyy";
        p2.AcceptPhotoAsBaseline();

        AllPatients.Add(p1);
        AllPatients.Add(p2);
    }

    private void PerformSearch()
    {
        SearchResults.Clear();

        var q = (SearchText ?? "").Trim();
        var src = string.IsNullOrWhiteSpace(q)
            ? AllPatients
            : new ObservableCollection<PatientVm>(
                AllPatients.Where(p =>
                    (p.FirstName ?? "").Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    (p.LastName ?? "").Contains(q, StringComparison.OrdinalIgnoreCase) ||
                    (p.PhoneNumber ?? "").Contains(q, StringComparison.OrdinalIgnoreCase)));

        foreach (var p in src)
            SearchResults.Add(p);

        SearchSelectedPatient = SearchResults.FirstOrDefault();
    }

    private void SelectSearchPatient()
    {
        if (SearchSelectedPatient == null) return;
        SelectedPatient = SearchSelectedPatient;
        IsSearchOpen = false;
    }

    private void NewPatient()
    {
        SelectedPatient = PatientVm.CreateNew();

        PendingBiopsies.Clear();
        PendingPathologyReports.Clear();
        PendingAppointments.Clear();
        PatientVisits.Clear();
        PatientAppointments.Clear();
        PatientEndoscopies.Clear();
        PatientPathologyReports.Clear();
    }

    private void SavePatient()
    {
        // dummy save: accept current photo as last saved
        SelectedPatient?.AcceptPhotoAsBaseline();
        RefreshPatientCommand.RaiseCanExecuteChanged();
    }

    private void DeletePatient()
    {
        if (SelectedPatient == null) return;

        AllPatients.Remove(SelectedPatient);
        SearchResults.Remove(SelectedPatient);
        SelectedPatient = AllPatients.FirstOrDefault();
    }

    private void CapturePhoto()
    {
        if (SelectedPatient == null) return;
        SelectedPatient.PhotoImageSource = DummyBitmapFactory.MakeGrayTile(256, 256);
        // IsPhotoDirty updates inside PatientVm
    }

    private void PhotoFromFile()
    {
        if (SelectedPatient == null) return;
        SelectedPatient.PhotoImageSource = DummyBitmapFactory.MakeGrayTile(256, 256);
    }

    private void LoadDummyDashboardForSelectedPatient()
    {
        PendingBiopsies.Clear();
        PendingPathologyReports.Clear();
        PendingAppointments.Clear();
        PatientVisits.Clear();
        PatientAppointments.Clear();
        PatientEndoscopies.Clear();
        PatientPathologyReports.Clear();

        if (SelectedPatient == null) return;

        PendingAppointments.Add(new PendingAppointmentVm
        {
            Date = DateTime.Today.AddDays(2).AddHours(10),
            IsUrgent = false,
            AppointmentTypeName = "Γαστροσκόπηση"
        });

        PatientAppointments.Add(new AppointmentVm
        {
            Start = DateTime.Today.AddDays(10).AddHours(9),
            Duration = TimeSpan.FromMinutes(60),
            PatientFullName = $"{SelectedPatient.LastName} {SelectedPatient.FirstName}",
            PatientPhoneNo = SelectedPatient.PhoneNumber ?? "",
            AppointmentTypeName = "Κολονοσκόπηση"
        });
    }

    private void OpenScheduler()
    {
        if (SelectedPatient == null) return;

        var vm = new NewAppointmentDayViewModel
        {
            SelectedPatient = SelectedPatient,
            SelectedDate = DateTime.Today
        };

        // dummy appointment types
        vm.AppointmentTypes.Add(new AppointmentTypeVm { Name = "Γαστροσκόπηση", MeanDurationMinutes = 30 });
        vm.AppointmentTypes.Add(new AppointmentTypeVm { Name = "Κολονοσκόπηση", MeanDurationMinutes = 60 });

        // dummy existing appointments for the day (blocks times)
        vm.AppointmentsOfSelectedDate.Add(new AppointmentVm
        {
            Start = DateTime.Today.Date.AddHours(10),
            Duration = TimeSpan.FromMinutes(60),
            PatientFullName = "ΔΗΜΗΤΡΗΣ ΚΩΝΣΤΑΝΤΙΝΟΥ",
            PatientPhoneNo = "69zzzzzzzz",
            AppointmentTypeName = "Κολονοσκόπηση"
        });

        AppointmentScheduler = vm;
        IsSchedulerOpen = true;
    }

    private void RaiseAllCanExecutes()
    {
        SavePatientCommand.RaiseCanExecuteChanged();
        DeletePatientCommand.RaiseCanExecuteChanged();
        CapturePhotoCommand.RaiseCanExecuteChanged();
        PhotoFromFileCommand.RaiseCanExecuteChanged();
        RefreshPatientCommand.RaiseCanExecuteChanged();
        NewAppointmentCommand.RaiseCanExecuteChanged();
    }
}
