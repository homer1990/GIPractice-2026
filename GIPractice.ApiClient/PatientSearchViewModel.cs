// PatientSearchViewModel.cs
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GIPractice.Api.Models;   // for PatientSearchRequestDto, PatientListItemDto

namespace GIPractice.Client;

public sealed class PatientSearchViewModel
    : PagedSearchViewModelBase<PatientListItemDto, PatientSearchRequestDto>
{
    private readonly IPatientsModule _patientsApi;

    public PatientSearchViewModel(IPatientsModule patientsApi)
    {
        _patientsApi = patientsApi;

        PageSize = 50;
        SortField = "LastVisit";
        SortDescending = true;
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

    // Keep compatibility with existing XAML: Patients => Items
    public ObservableCollection<PatientListItemDto> Patients => Items;

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
        // Whatever your client looks like – adjust method name/signature
        return _patientsApi.SearchAsync(request);
    }
}
