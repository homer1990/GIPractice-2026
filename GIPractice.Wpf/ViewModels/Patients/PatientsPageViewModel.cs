using System.Collections.ObjectModel;
using System.ComponentModel;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace GIPractice.Wpf.ViewModels;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public sealed class PatientsPageViewModel : ViewModelBase
{
    private readonly ICalendarDayMetaStore _dayStore = new InMemoryCalendarDayMetaStore();

    private bool _isSchedulerOpen;
    private NewAppointmentDayViewModel? _appointmentScheduler;

    private PatientVm? _selectedPatient;
    private bool _isSearchOpen;

    private string? _searchFirstName;
    private string? _searchLastName;
    private string? _searchFathersName;
    private string? _searchPersonalNumber;
    private string? _searchPhone;
    private string? _searchEmail;
    private bool _searchHasHadCA;
    private bool _searchHasHadIBD;
    private bool _searchHasPendingBiopsies;
    private bool _searchHasScheduledEndo;
    private PatientVm? _searchSelectedPatient;

    private EncounterHistoryVm? _selectedHistoryEncounter;

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

    public bool IsSearchOpen
    {
        get => _isSearchOpen;
        set => SetProperty(ref _isSearchOpen, value);
    }

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

    public EncounterHistoryVm? SelectedHistoryEncounter
    {
        get => _selectedHistoryEncounter;
        set => SetProperty(ref _selectedHistoryEncounter, value);
    }

    // Explicit field-by-field search (no live filtering for performance).
    public string? SearchFirstName
    {
        get => _searchFirstName;
        set => SetProperty(ref _searchFirstName, value);
    }

    public string? SearchLastName
    {
        get => _searchLastName;
        set => SetProperty(ref _searchLastName, value);
    }

    public string? SearchFathersName
    {
        get => _searchFathersName;
        set => SetProperty(ref _searchFathersName, value);
    }

    public string? SearchPersonalNumber
    {
        get => _searchPersonalNumber;
        set => SetProperty(ref _searchPersonalNumber, value);
    }

    public string? SearchPhone
    {
        get => _searchPhone;
        set => SetProperty(ref _searchPhone, value);
    }

    public string? SearchEmail
    {
        get => _searchEmail;
        set => SetProperty(ref _searchEmail, value);
    }

    public bool SearchHasHadCA
    {
        get => _searchHasHadCA;
        set => SetProperty(ref _searchHasHadCA, value);
    }

    public bool SearchHasHadIBD
    {
        get => _searchHasHadIBD;
        set => SetProperty(ref _searchHasHadIBD, value);
    }

    public bool SearchHasPendingBiopsies
    {
        get => _searchHasPendingBiopsies;
        set => SetProperty(ref _searchHasPendingBiopsies, value);
    }

    public bool SearchHasScheduledEndo
    {
        get => _searchHasScheduledEndo;
        set => SetProperty(ref _searchHasScheduledEndo, value);
    }

    public ObservableCollection<PatientVm> AllPatients { get; } = [];
    public ObservableCollection<PatientVm> SearchResults { get; } = [];

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
        ["Άρρεν", "Θήλυ", "Άλλο..."];

    // Dummy dashboard collections
    public ObservableCollection<PendingBiopsyVm> PendingBiopsies { get; } = [];
    public ObservableCollection<PendingPathologyVm> PendingPathologyReports { get; } = [];
    public ObservableCollection<PendingAppointmentVm> PendingAppointments { get; } = [];
    public ObservableCollection<VisitVm> PatientVisits { get; } = [];

    // Continuity: reuse AppointmentVm from scheduler work
    public ObservableCollection<AppointmentVm> PatientAppointments { get; } = [];

    // Patient history: encounters (real-world happened). If EndoscopyTypeName is set, UI shows that instead of EncounterTypeName.
    public ObservableCollection<EncounterHistoryVm> PatientHistoryEncounters { get; } = [];

    public ObservableCollection<EndoscopyVm> PatientEndoscopies { get; } = [];
    public ObservableCollection<PathologyReportVm> PatientPathologyReports { get; } = [];

    // Commands
    public RelayCommand ToggleSearchCommand { get; }
    public RelayCommand PerformSearchCommand { get; }
    public RelayCommand ClearSearchCommand { get; }
    public RelayCommand SelectSearchPatientCommand { get; }

    public RelayCommand NewPatientCommand { get; }
    public RelayCommand SavePatientCommand { get; }
    public RelayCommand DeletePatientCommand { get; }

    public RelayCommand CapturePhotoCommand { get; }
    public RelayCommand PhotoFromFileCommand { get; }
    public RelayCommand RefreshPatientCommand { get; } // ΕΠΑΝΑΦΟΡΑ: reset photo baseline

    public RelayCommand<AppointmentVm> EditPatientAppointmentCommand { get; }
    public RelayCommand<AppointmentVm> DeletePatientAppointmentCommand { get; }

    public RelayCommand<EncounterHistoryVm> OpenEncounterCommand { get; }

    // fields
    private readonly RelayCommand _newAppointmentCommand;
    private readonly RelayCommand _closeSchedulerCommand;

    // properties
    public RelayCommand NewAppointmentCommand => _newAppointmentCommand;
    public RelayCommand CloseSchedulerCommand => _closeSchedulerCommand;

    public PatientsPageViewModel()
    {
        OpenEncounterCommand = new RelayCommand<EncounterHistoryVm>(OpenEncounter);

        ToggleSearchCommand = new RelayCommand(() => IsSearchOpen = !IsSearchOpen);
        PerformSearchCommand = new RelayCommand(PerformSearch);
        ClearSearchCommand = new RelayCommand(ClearSearch);
        SelectSearchPatientCommand = new RelayCommand(SelectSearchPatient, () => SearchSelectedPatient != null);

        NewPatientCommand = new RelayCommand(NewPatient);
        SavePatientCommand = new RelayCommand(SavePatient, () => SelectedPatient != null);
        DeletePatientCommand = new RelayCommand(DeletePatient, () => SelectedPatient != null);

        CapturePhotoCommand = new RelayCommand(CapturePhoto, () => SelectedPatient != null);
        PhotoFromFileCommand = new RelayCommand(PhotoFromFile, () => SelectedPatient != null);

        RefreshPatientCommand = new RelayCommand(
            () => SelectedPatient?.ResetPhotoToBaseline(),
            () => SelectedPatient != null && SelectedPatient.IsPhotoDirty);

        EditPatientAppointmentCommand = new RelayCommand<AppointmentVm>(EditPatientAppointment);
        DeletePatientAppointmentCommand = new RelayCommand<AppointmentVm>(DeletePatientAppointment);

        _newAppointmentCommand = new RelayCommand(OpenScheduler, () => SelectedPatient != null);
        _closeSchedulerCommand = new RelayCommand(() => IsSchedulerOpen = false);

        SeedDummyPatients();
        ClearSearch(); // resets filters + calls PerformSearch()
        SelectedPatient = AllPatients.FirstOrDefault();
    }

    private void OpenEncounter(EncounterHistoryVm row)
    {
        SelectedHistoryEncounter = row;

        // Later routing rule:
        // if (row.EndoscopyTypeName != null) open Endoscopy details
        // else open Encounter details
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
        p1.Id = 1;
        p1.FirstName = "Γιώργος";
        p1.LastName = "Παπαδόπουλος";
        p1.PhoneNumber = "69xxxxxxxx";
        p1.HasHadIBD = true;
        p1.AcceptPhotoAsBaseline();

        var p2 = PatientVm.CreateNew();
        p2.Id = 2;
        p2.FirstName = "Μαρία";
        p2.LastName = "Ιωάννου";
        p2.PhoneNumber = "69yyyyyyyy";
        p2.HasPendingBiopsies = true;
        p2.HasScheduledEndo = true;
        p2.AcceptPhotoAsBaseline();

        AllPatients.Add(p1);
        AllPatients.Add(p2);
    }

    private void PerformSearch()
    {
        SearchResults.Clear();

        static string Norm(string? s) => (s ?? "").Trim();
        static bool Has(string? s) => !string.IsNullOrWhiteSpace(s);

        static bool Contains(string? hay, string needle) =>
            (hay ?? "").Contains(needle, StringComparison.OrdinalIgnoreCase);

        var fn = Norm(SearchFirstName);
        var ln = Norm(SearchLastName);
        var fat = Norm(SearchFathersName);
        var pn = Norm(SearchPersonalNumber);
        var ph = Norm(SearchPhone);
        var em = Norm(SearchEmail);

        var src = new ObservableCollection<PatientVm>(
            AllPatients.Where(p =>
                (!Has(fn) || Contains(p.FirstName, fn)) &&
                (!Has(ln) || Contains(p.LastName, ln)) &&
                (!Has(fat) || Contains(p.FathersName, fat)) &&
                (!Has(pn) || Contains(p.PersonalNumber, pn)) &&
                (!Has(ph) || Contains(p.PhoneNumber, ph)) &&
                (!Has(em) || Contains(p.Email, em)) &&
                (!SearchHasHadCA || p.HasHadCA) &&
                (!SearchHasHadIBD || p.HasHadIBD) &&
                (!SearchHasPendingBiopsies || p.HasPendingBiopsies) &&
                (!SearchHasScheduledEndo || p.HasScheduledEndo)));

        foreach (var p in src)
            SearchResults.Add(p);

        SearchSelectedPatient = SearchResults.FirstOrDefault();
    }

    private void ClearSearch()
    {
        SearchFirstName = null;
        SearchLastName = null;
        SearchFathersName = null;
        SearchPersonalNumber = null;
        SearchPhone = null;
        SearchEmail = null;

        SearchHasHadCA = false;
        SearchHasHadIBD = false;
        SearchHasPendingBiopsies = false;
        SearchHasScheduledEndo = false;

        PerformSearch();
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
        PatientHistoryEncounters.Clear();
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
        PatientHistoryEncounters.Clear();
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

        // Patient history / encounters (dummy)
        PatientHistoryEncounters.Add(new EncounterHistoryVm
        {
            Start = DateTime.Today.AddDays(-7).AddHours(11),
            End = DateTime.Today.AddDays(-7).AddHours(11).AddMinutes(20),
            EncounterTypeName = "Ιατρείο",
            Notes = "Κλινική εξέταση"
        });

        // Endoscopy encounter: show endoscopy type in UI
        PatientHistoryEncounters.Add(new EncounterHistoryVm
        {
            Start = DateTime.Today.AddDays(-30).AddHours(9),
            End = DateTime.Today.AddDays(-30).AddHours(10),
            EncounterTypeName = "Ενδοσκόπηση",
            EndoscopyTypeName = "Γαστροσκόπηση",
            Notes = "Ήπια γαστρίτιδα"
        });
    }

    private void OpenScheduler()
    {
        if (SelectedPatient == null) return;

        var vm = new NewAppointmentDayViewModel(_dayStore)
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

    private void EditPatientAppointment(AppointmentVm appt)
    {
        // Dummy implementation for now.
        // Later: open scheduler/details with this appointment pre-selected.
        IsSchedulerOpen = true;
    }

    private void DeletePatientAppointment(AppointmentVm appt)
    {
        PatientAppointments.Remove(appt);
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
