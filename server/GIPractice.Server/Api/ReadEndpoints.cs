using GIPractice.Server.Data;
using GIPractice.Server.Patients;
using GIPractice.Server.Scheduling;

namespace GIPractice.Server.Api;

public static class ReadEndpoints
{
    public static IEndpointRouteBuilder MapPracticeReadApi(this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.MapGroup("/api");

        api.MapGet("/patients/search", SearchPatients);
        api.MapGet("/patients/{id:guid}", GetPatient);
        api.MapGet("/appointments", GetAppointments);

        return endpoints;
    }

    private static IResult SearchPatients(
        InMemoryPracticeReadStore store,
        string? firstName,
        string? lastName,
        string? fathersName,
        DateOnly? birthDateFrom,
        DateOnly? birthDateTo,
        int page = 1,
        int pageSize = 50)
    {
        if (page < 1)
            return Results.BadRequest(new { error = "page must be at least 1." });

        if (pageSize is < 1 or > 200)
            return Results.BadRequest(new { error = "pageSize must be between 1 and 200." });

        if (birthDateFrom is not null && birthDateTo is not null && birthDateFrom > birthDateTo)
            return Results.BadRequest(new { error = "birthDateFrom must not be after birthDateTo." });

        var result = store.SearchPatients(
            firstName,
            lastName,
            fathersName,
            birthDateFrom,
            birthDateTo,
            page,
            pageSize);

        return Results.Ok(new PatientSearchResponseDto(
            result.Items.Select(ToListItem).ToList(),
            result.TotalCount,
            page,
            pageSize));
    }

    private static IResult GetPatient(InMemoryPracticeReadStore store, Guid id)
    {
        var patient = store.GetPatient(id);
        return patient is null
            ? Results.NotFound()
            : Results.Ok(ToDetails(patient));
    }

    private static IResult GetAppointments(
        InMemoryPracticeReadStore store,
        DateTimeOffset? fromUtc,
        DateTimeOffset? toUtc,
        Guid? patientId)
    {
        if (fromUtc is null || toUtc is null)
            return Results.BadRequest(new { error = "fromUtc and toUtc are required." });

        if (fromUtc >= toUtc)
            return Results.BadRequest(new { error = "fromUtc must be before toUtc." });

        var appointments = store.GetAppointments(fromUtc.Value, toUtc.Value, patientId);
        var items = new List<AppointmentListItemDto>(appointments.Count);

        foreach (var appointment in appointments)
        {
            var patient = store.GetPatient(appointment.PatientId);
            var displayName = patient is null
                ? appointment.PatientId.ToString()
                : $"{patient.LastName} {patient.FirstName}";

            items.Add(ToListItem(appointment, displayName));
        }

        return Results.Ok(items);
    }

    private static PatientListItemDto ToListItem(Patient patient) =>
        new(patient.Id, patient.FirstName, patient.LastName, patient.FathersName, patient.BirthDate);

    private static PatientDetailsDto ToDetails(Patient patient) =>
        new(patient.Id, patient.FirstName, patient.LastName, patient.FathersName, patient.BirthDate);

    private static AppointmentListItemDto ToListItem(Appointment appointment, string patientDisplayName) =>
        new(
            appointment.Id,
            appointment.PatientId,
            patientDisplayName,
            appointment.TypeCode,
            appointment.StartUtc,
            appointment.DurationMinutes,
            appointment.Status.ToString(),
            appointment.Notes);
}
