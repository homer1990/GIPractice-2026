using GIPractice.Api.Models;

namespace GIPractice.Client;

public class GiPracticeApiClient(ClientController controller)
{
    private readonly ClientController _controller = controller;

    public async Task<LocalizationResponse?> GetLocalizationAsync(string table, string field, string culture,
        CancellationToken cancellationToken = default)
    {
        var url = $"api/localization/{Uri.EscapeDataString(table)}/{Uri.EscapeDataString(field)}?culture={Uri.EscapeDataString(culture)}";
        return await _http.GetFromJsonAsync<LocalizationResponse>(url, cancellationToken);
    }

    // -----------------------
    // PATIENT SEARCH & DASHBOARD
    // -----------------------

    public async Task<PagedResultDto<PatientListItemDto>> SearchPatientsAsync(
        PatientSearchRequestDto request,
        CancellationToken cancellationToken = default)
    {
        // Build query string from request
        var qs = new List<string>();

        void Add(string name, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                qs.Add($"{Uri.EscapeDataString(name)}={Uri.EscapeDataString(value)}");
        }

        void AddNullableDate(string name, DateTime? value)
        {
            if (value.HasValue)
                qs.Add($"{Uri.EscapeDataString(name)}={Uri.EscapeDataString(value.Value.ToUniversalTime().ToString("O"))}");
        }

        Add("PersonalNumber", request.PersonalNumber);
        Add("LastName", request.LastName);
        Add("FirstName", request.FirstName);
        Add("FathersName", request.FathersName);
        AddNullableDate("BirthDateFrom", request.BirthDateFrom);
        AddNullableDate("BirthDateTo", request.BirthDateTo);
        Add("PhoneNumber", request.PhoneNumber);
        Add("Email", request.Email);

        qs.Add($"Page={request.Page}");
        qs.Add($"PageSize={request.PageSize}");

        var url = "api/patients/search";
        if (qs.Count > 0)
            url += "?" + string.Join("&", qs);

        var result = await _controller.GetAsync<PagedResultDto<PatientListItemDto>>(url, cancellationToken);

        return result ?? new PagedResultDto<PatientListItemDto>
        {
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = 0,
            Items = []
        };
    }

    public async Task<PatientDashboardDto?> GetPatientDashboardAsync(
        int patientId,
        CancellationToken cancellationToken = default)
    {
        return await _controller.GetAsync<PatientDashboardDto>(
            $"api/patients/{patientId}/dashboard",
            cancellationToken);
    }

    public async Task<PatientSummaryDto?> GetPatientSummaryAsync(
        int patientId,
        CancellationToken cancellationToken = default)
    {
        return await _controller.GetAsync<PatientSummaryDto>(
            $"api/patients/{patientId}/summary",
            cancellationToken);
    }

    public async Task<List<PatientTimelineItemDto>> GetPatientTimelineAsync(
        int patientId,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var qs = new List<string>();
        if (fromUtc.HasValue)
            qs.Add($"fromUtc={Uri.EscapeDataString(fromUtc.Value.ToUniversalTime().ToString("O"))}");
        if (toUtc.HasValue)
            qs.Add($"toUtc={Uri.EscapeDataString(toUtc.Value.ToUniversalTime().ToString("O"))}");

        var url = $"api/patients/{patientId}/timeline";
        if (qs.Count > 0)
            url += "?" + string.Join("&", qs);

        var result = await _controller.GetAsync<List<PatientTimelineItemDto>>(
            url, cancellationToken);

        return result ?? [];
    }

    // -----------------------
    // SCHEDULE
    // -----------------------

    public async Task<ScheduleRangeDto?> GetScheduleAsync(
        DateTime fromUtc,
        DateTime toUtc,
        int? patientId = null,
        CancellationToken cancellationToken = default)
    {
        var qs = new List<string>
        {
            $"fromUtc={Uri.EscapeDataString(fromUtc.ToUniversalTime().ToString("O"))}",
            $"toUtc={Uri.EscapeDataString(toUtc.ToUniversalTime().ToString("O"))}"
        };

        if (patientId.HasValue)
            qs.Add($"patientId={patientId.Value}");

        var url = "api/schedule?" + string.Join("&", qs);

        return await _controller.GetAsync<ScheduleRangeDto>(url, cancellationToken);
    }

    // -----------------------
    // VISITS
    // -----------------------

    public async Task<VisitDetailsDto?> GetVisitDetailsAsync(
        int visitId,
        CancellationToken cancellationToken = default)
    {
        return await _controller.GetAsync<VisitDetailsDto>(
            $"api/visits/{visitId}/details",
            cancellationToken);
    }

    public async Task<List<VisitDto>> GetPatientVisitsAsync(
        int patientId,
        CancellationToken cancellationToken = default)
    {
        var result = await _controller.GetAsync<List<VisitDto>>(
            $"api/patients/{patientId}/visits",
            cancellationToken);

        return result ?? [];
    }

    // -----------------------
    // ENDOSCOPIES
    // -----------------------

    public async Task<List<EndoscopyListItemDto>> GetEndoscopiesAsync(
        int? patientId = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var qs = new List<string>();
        if (patientId.HasValue) qs.Add($"patientId={patientId.Value}");
        if (fromUtc.HasValue)
            qs.Add($"fromUtc={Uri.EscapeDataString(fromUtc.Value.ToUniversalTime().ToString("O"))}");
        if (toUtc.HasValue)
            qs.Add($"toUtc={Uri.EscapeDataString(toUtc.Value.ToUniversalTime().ToString("O"))}");

        var url = "api/endoscopies";
        if (qs.Count > 0)
            url += "?" + string.Join("&", qs);

        var result = await _controller.GetAsync<List<EndoscopyListItemDto>>(
            url, cancellationToken);

        return result ?? [];
    }

    public async Task<List<EndoscopyDto>> GetPatientEndoscopiesAsync(
        int patientId,
        CancellationToken cancellationToken = default)
    {
        var result = await _controller.GetAsync<List<EndoscopyDto>>(
            $"api/patients/{patientId}/endoscopies",
            cancellationToken);

        return result ?? [];
    }

    public async Task<EndoscopyDto?> GetEndoscopyAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _controller.GetAsync<EndoscopyDto>(
            $"api/endoscopies/{id}",
            cancellationToken);
    }

    // -----------------------
    // TESTS / OPERATIONS / INFAI
    // -----------------------

    public async Task<List<TestListItemDto>> GetTestsAsync(
        int? patientId = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var qs = new List<string>();
        if (patientId.HasValue) qs.Add($"patientId={patientId.Value}");
        if (fromUtc.HasValue)
            qs.Add($"fromUtc={Uri.EscapeDataString(fromUtc.Value.ToUniversalTime().ToString("O"))}");
        if (toUtc.HasValue)
            qs.Add($"toUtc={Uri.EscapeDataString(toUtc.Value.ToUniversalTime().ToString("O"))}");

        var url = "api/tests/list";
        if (qs.Count > 0)
            url += "?" + string.Join("&", qs);

        var result = await _controller.GetAsync<List<TestListItemDto>>(url, cancellationToken);
        return result ?? [];
    }

    public async Task<List<OperationListItemDto>> GetOperationsAsync(
        int? patientId = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var qs = new List<string>();
        if (patientId.HasValue) qs.Add($"patientId={patientId.Value}");
        if (fromUtc.HasValue)
            qs.Add($"fromUtc={Uri.EscapeDataString(fromUtc.Value.ToUniversalTime().ToString("O"))}");
        if (toUtc.HasValue)
            qs.Add($"toUtc={Uri.EscapeDataString(toUtc.Value.ToUniversalTime().ToString("O"))}");

        var url = "api/operations/list";
        if (qs.Count > 0)
            url += "?" + string.Join("&", qs);

        var result = await _controller.GetAsync<List<OperationListItemDto>>(url, cancellationToken);
        return result ?? [];
    }

    public async Task<List<InfaiTestListItemDto>> GetInfaiTestsAsync(
        int? patientId = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var qs = new List<string>();
        if (patientId.HasValue) qs.Add($"patientId={patientId.Value}");
        if (fromUtc.HasValue)
            qs.Add($"fromUtc={Uri.EscapeDataString(fromUtc.Value.ToUniversalTime().ToString("O"))}");
        if (toUtc.HasValue)
            qs.Add($"toUtc={Uri.EscapeDataString(toUtc.Value.ToUniversalTime().ToString("O"))}");

        var url = "api/infaitests/list";
        if (qs.Count > 0)
            url += "?" + string.Join("&", qs);

        var result = await _controller.GetAsync<List<InfaiTestListItemDto>>(url, cancellationToken);
        return result ?? [];
    }

    // -----------------------
    // APPOINTMENTS
    // -----------------------

    public async Task<List<AppointmentDto>> GetPatientAppointmentsAsync(
        int patientId,
        CancellationToken cancellationToken = default)
    {
        var result = await _controller.GetAsync<List<AppointmentDto>>(
            $"api/patients/{patientId}/appointments",
            cancellationToken);

        return result ?? [];
    }
}
