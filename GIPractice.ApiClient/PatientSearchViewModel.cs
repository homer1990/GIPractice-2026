// PatientSearchViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GIPractice.Api.Models;   // for PatientSearchRequestDto, PatientListItemDto

namespace GIPractice.Client;

public sealed class PatientSearchViewModel
    : SearchViewModelBase<PatientListItemDto, PatientSearchRequestDto>
{
    private readonly IPatientsModule _patientsApi;
    private readonly INavigationService _navigation;

    private PatientListItemDto? _selectedPatient;

    public PatientSearchViewModel(IPatientsModule patientsApi, INavigationService navigation)
        : base(navigation)
    {
        _patientsApi = patientsApi;
        _navigation = navigation;

        PageSize = 50;
        SortField = "LastVisit";
        SortDescending = true;
    }

    public PatientListItemDto? SelectedPatient
    {
        get => _selectedPatient;
        set
        {
            if (SetProperty(ref _selectedPatient, value))
                OnSelectedPatientChanged();
        }
    }

    // Filters bound from XAML
    private string? _lastName;
    public string? LastName
    {
        get => _lastName;
        set => SetProperty(ref _lastName, value);
    }

    private string? _firstName;
    public string? FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }

    private string? _fathersName;
    public string? FathersName
    {
        get => _fathersName;
        set => SetProperty(ref _fathersName, value);
    }

    private string? _personalNumber;
    public string? PersonalNumber
    {
        get => _personalNumber;
        set => SetProperty(ref _personalNumber, value);
    }

    private DateTime? _birthFrom;
    public DateTime? BirthDateFrom
    {
        get => _birthFrom;
        set => SetProperty(ref _birthFrom, value);
    }

    private DateTime? _birthTo;
    public DateTime? BirthDateTo
    {
        get => _birthTo;
        set => SetProperty(ref _birthTo, value);
    }

    private string? _phoneNumber;
    public string? PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty(ref _phoneNumber, value);
    }

    private string? _email;
    public string? Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public ObservableCollection<PatientListItemDto> Patients => Items;

    protected override void OnPropertyChanged(string? name = null)
    {
        base.OnPropertyChanged(name);

        if (name == nameof(SelectedItem))
            SelectedPatient = SelectedItem;
    }

    protected override PatientSearchRequestDto BuildRequestCore()
    {
        return new PatientSearchRequestDto
        {
            PersonalNumber = PersonalNumber,
            LastName = LastName,
            FirstName = FirstName,
            FathersName = FathersName,
            BirthDateFrom = BirthDateFrom,
            BirthDateTo = BirthDateTo,
            PhoneNumber = PhoneNumber,
            Email = Email,
            // Page/PageSize/Sort* are filled in base via dynamic r = request
        };
    }

    protected override Task<PagedResultDto<PatientListItemDto>> ExecuteSearchAsync(
        PatientSearchRequestDto request)
    {
        return _patientsApi.SearchAsync(request);
    }

    protected override Task NavigateAsync(PatientListItemDto item)
    {
        return _navigation.ShowPatientDashboardAsync(item);
    }

    private void OnSelectedPatientChanged()
    {
        SelectedItem = SelectedPatient;
    }
}
