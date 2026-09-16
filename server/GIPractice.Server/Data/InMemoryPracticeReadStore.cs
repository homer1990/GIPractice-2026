using GIPractice.Server.Patients;
using GIPractice.Server.Scheduling;

namespace GIPractice.Server.Data;

// Temporary read store used to prove the HTTP/client contract before SQLite is introduced.
// It is intentionally small and synthetic; it must not become the production data source.
public sealed class InMemoryPracticeReadStore
{
    private readonly IReadOnlyList<Patient> _patients;
    private readonly IReadOnlyList<Appointment> _appointments;

    public InMemoryPracticeReadStore()
    {
        var patient1 = new Patient(
            Guid.Parse("11111111-1111-7111-8111-111111111111"),
            "Δημήτριος",
            "Παπαδόπουλος",
            "Νικόλαος",
            new DateOnly(1968, 4, 12));

        var patient2 = new Patient(
            Guid.Parse("22222222-2222-7222-8222-222222222222"),
            "Μαρία",
            "Νικολάου",
            "Γεώργιος",
            new DateOnly(1977, 11, 3));

        var patient3 = new Patient(
            Guid.Parse("33333333-3333-7333-8333-333333333333"),
            "Ιωάννης",
            "Γεωργίου",
            null,
            new DateOnly(1985, 7, 21));

        _patients = [patient1, patient2, patient3];

        var todayUtc = new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero);
        _appointments =
        [
            new Appointment(
                Guid.Parse("aaaaaaaa-aaaa-7aaa-8aaa-aaaaaaaaaaaa"),
                patient1.Id,
                "GASTROSCOPY",
                todayUtc.AddHours(8),
                30,
                AppointmentStatus.Scheduled,
                "Development data"),
            new Appointment(
                Guid.Parse("bbbbbbbb-bbbb-7bbb-8bbb-bbbbbbbbbbbb"),
                patient2.Id,
                "COLONOSCOPY",
                todayUtc.AddHours(9),
                45,
                AppointmentStatus.Arrived),
            new Appointment(
                Guid.Parse("cccccccc-cccc-7ccc-8ccc-cccccccccccc"),
                patient1.Id,
                "COLONOSCOPY",
                todayUtc.AddDays(1).AddHours(8),
                45)
        ];
    }

    public Patient? GetPatient(Guid id) => _patients.FirstOrDefault(patient => patient.Id == id);

    public (IReadOnlyList<Patient> Items, int TotalCount) SearchPatients(
        string? firstName,
        string? lastName,
        string? fathersName,
        DateOnly? birthDateFrom,
        DateOnly? birthDateTo,
        int page,
        int pageSize)
    {
        IEnumerable<Patient> query = _patients;

        query = FilterContains(query, firstName, patient => patient.FirstName);
        query = FilterContains(query, lastName, patient => patient.LastName);
        query = FilterContains(query, fathersName, patient => patient.FathersName);

        if (birthDateFrom is not null)
            query = query.Where(patient => patient.BirthDate is not null && patient.BirthDate.Value >= birthDateFrom.Value);

        if (birthDateTo is not null)
            query = query.Where(patient => patient.BirthDate is not null && patient.BirthDate.Value <= birthDateTo.Value);

        var ordered = query
            .OrderBy(patient => patient.LastName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(patient => patient.FirstName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var totalCount = ordered.Count;
        var items = ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (items, totalCount);
    }

    public IReadOnlyList<Appointment> GetAppointments(
        DateTimeOffset fromUtc,
        DateTimeOffset toUtc,
        Guid? patientId)
    {
        var from = fromUtc.ToUniversalTime();
        var to = toUtc.ToUniversalTime();

        return _appointments
            .Where(appointment => appointment.StartUtc >= from && appointment.StartUtc < to)
            .Where(appointment => patientId is null || appointment.PatientId == patientId.Value)
            .OrderBy(appointment => appointment.StartUtc)
            .ToList();
    }

    private static IEnumerable<Patient> FilterContains(
        IEnumerable<Patient> source,
        string? searchText,
        Func<Patient, string?> selector)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return source;

        var term = searchText.Trim();
        return source.Where(patient =>
        {
            var value = selector(patient);
            return value is not null && value.Contains(term, StringComparison.OrdinalIgnoreCase);
        });
    }
}
