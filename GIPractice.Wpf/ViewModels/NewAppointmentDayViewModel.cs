using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace GIPractice.Wpf.ViewModels;

public sealed class NewAppointmentDayViewModel : ViewModelBase
{
    private readonly ICalendarDayMetaStore _dayStore;

    private DayMetaVm? _currentDay;
    public DayMetaVm? CurrentDay
    {
        get => _currentDay;
        private set
        {
            if (_currentDay != null)
                _currentDay.PropertyChanged -= CurrentDay_PropertyChanged;

            if (!SetProperty(ref _currentDay, value))
                return;

            if (_currentDay != null)
                _currentDay.PropertyChanged += CurrentDay_PropertyChanged;

            OnPropertyChanged(nameof(CanSchedule));
            RebuildAvailableStartTimes();
            AddAppointmentCommand.RaiseCanExecuteChanged();
        }
    }

    public bool CanSchedule => CurrentDay?.IsClosed == false;
    private static readonly TimeSpan ClinicOpen = TimeSpan.FromHours(8);
    private static readonly TimeSpan ClinicClose = TimeSpan.FromHours(22);
    private static readonly TimeSpan SlotStep = TimeSpan.FromMinutes(30);

    private PatientVm? _selectedPatient;
    private DateTime _selectedDate = DateTime.Today;

    private AppointmentTypeVm? _newAppointmentType;
    private TimeSpan? _newStartTime;

    private AppointmentVm? _selectedAppointment;

    public PatientVm? SelectedPatient
    {
        get => _selectedPatient;
        set
        {
            if (!SetProperty(ref _selectedPatient, value))
                return;
            AddAppointmentCommand.RaiseCanExecuteChanged();
        }
    }

    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            // keep date portion sane (your UI uses date formatting everywhere)
            var v = value.Date;
            if (!SetProperty(ref _selectedDate, v))
                return;

            RebuildAvailableStartTimes();
            // later: reload AppointmentsOfSelectedDate from API for this date
        }
    }

    public ObservableCollection<AppointmentVm> AppointmentsOfSelectedDate { get; } = new();
    public ObservableCollection<AppointmentTypeVm> AppointmentTypes { get; } = new();

    public AppointmentTypeVm? NewAppointmentType
    {
        get => _newAppointmentType;
        set
        {
            if (!SetProperty(ref _newAppointmentType, value))
                return;

            RebuildAvailableStartTimes();
            AddAppointmentCommand.RaiseCanExecuteChanged();
        }
    }

    public ObservableCollection<TimeSpan> AvailableStartTimes { get; } = new();

    public TimeSpan? NewStartTime
    {
        get => _newStartTime;
        set
        {
            if (!SetProperty(ref _newStartTime, value))
                return;
            AddAppointmentCommand.RaiseCanExecuteChanged();
        }
    }

    public AppointmentVm? SelectedAppointment
    {
        get => _selectedAppointment;
        set => SetProperty(ref _selectedAppointment, value);
    }

    public RelayCommand SearchPatientCommand { get; }
    public RelayCommand AddAppointmentCommand { get; }
    public RelayCommand<AppointmentVm> EditAppointmentCommand { get; }
    public RelayCommand<AppointmentVm> DeleteAppointmentCommand { get; }

    public NewAppointmentDayViewModel(ICalendarDayMetaStore? dayStore = null)
    {
        SearchPatientCommand = new RelayCommand(SearchPatient);
        AddAppointmentCommand = new RelayCommand(AddAppointment, CanAddAppointment);
        EditAppointmentCommand = new RelayCommand<AppointmentVm>(EditAppointment);
        DeleteAppointmentCommand = new RelayCommand<AppointmentVm>(DeleteAppointment);

        AppointmentsOfSelectedDate.CollectionChanged += (_, __) => RebuildAvailableStartTimes();

        _dayStore = dayStore ?? new InMemoryCalendarDayMetaStore();
    }
    private void CurrentDay_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(DayMetaVm.IsDayOff) or nameof(DayMetaVm.IsHoliday) or nameof(DayMetaVm.IsClosed))
        {
            OnPropertyChanged(nameof(CanSchedule));
            RebuildAvailableStartTimes();
            AddAppointmentCommand.RaiseCanExecuteChanged();
        }
    }
    private bool CanAddAppointment()
    => SelectedPatient != null
       && NewAppointmentType != null
       && NewStartTime != null
       && CanSchedule;
    private void SearchPatient()
    {
        // dummy: scheduler is usually opened from Patients with SelectedPatient already set.
        // later: call a dialog/search service.
    }

    private void AddAppointment()
    {
        if (!CanAddAppointment()) return;

        var start = SelectedDate.Date + NewStartTime!.Value;
        var duration = TimeSpan.FromMinutes(NewAppointmentType!.MeanDurationMinutes);

        if (!IsIntervalAvailable(start, duration))
            return; // should be impossible if AvailableStartTimes is correct

        AppointmentsOfSelectedDate.Add(new AppointmentVm
        {
            Start = start,
            Duration = duration,
            PatientFullName = $"{SelectedPatient!.LastName} {SelectedPatient.FirstName}",
            PatientPhoneNo = SelectedPatient.PhoneNumber ?? "",
            AppointmentTypeName = NewAppointmentType.Name,
            TypeAccentBrush = NewAppointmentType.AccentBrush
        });

        RebuildAvailableStartTimes();
        if (AvailableStartTimes.Count > 0)
            NewStartTime = AvailableStartTimes[0];
    }

    private void EditAppointment(AppointmentVm appt)
    {
        // dummy: later open editor dialog.
        // For now you could just do nothing.
    }

    private void DeleteAppointment(AppointmentVm appt)
    {
        AppointmentsOfSelectedDate.Remove(appt);
    }

    private void RebuildAvailableStartTimes()
    {
        AvailableStartTimes.Clear();

        if (!CanSchedule || NewAppointmentType == null)
        {
            NewStartTime = null;
            return;
        }

        var duration = TimeSpan.FromMinutes(NewAppointmentType.MeanDurationMinutes);

        for (var t = ClinicOpen; t + duration <= ClinicClose; t += SlotStep)
        {
            var start = SelectedDate.Date + t;
            if (IsIntervalAvailable(start, duration))
                AvailableStartTimes.Add(t);
        }

        if (NewStartTime.HasValue && !AvailableStartTimes.Contains(NewStartTime.Value))
            NewStartTime = null;
    }

    private bool IsIntervalAvailable(DateTime candidateStart, TimeSpan candidateDuration)
    {
        var candidateEnd = candidateStart + candidateDuration;

        foreach (var a in AppointmentsOfSelectedDate)
        {
            var aStart = a.Start;
            var aEnd = a.Start + a.Duration;

            // overlap: [start,end) intersects [aStart,aEnd)
            if (candidateStart < aEnd && candidateEnd > aStart)
                return false;
        }

        return true;
    }
}
