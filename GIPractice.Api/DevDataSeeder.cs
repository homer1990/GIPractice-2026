using GIPractice.Core.Entities;
using GIPractice.Core.Enums;
using GIPractice.Core.ValueObjects;
using GIPractice.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GIPractice.Api.Infrastructure;

public class DevDataSeeder
{
    private readonly AppDbContext _db;
    private readonly ILogger<DevDataSeeder> _logger;
    private readonly Random _random = new();

    public DevDataSeeder(AppDbContext db, ILogger<DevDataSeeder> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task SeedAsync(
        int targetPatientCount = 1000,
        int minVisitsPerPatient = 10,
        int maxVisitsPerPatient = 20,
        int minAppointmentsPerPatient = 15,
        int maxAppointmentsPerPatient = 30,
        int minEndoscopiesPerPatient = 2,
        int maxEndoscopiesPerPatient = 3,
        CancellationToken cancellationToken = default)
    {
        // Don’t reseed if we already have data
        if (await _db.Patients.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("DevDataSeeder: Patients already exist, skipping seeding.");
            return;
        }

        var patients = new List<Patient>();
        var appointments = new List<Appointment>();
        var visits = new List<Visit>();
        var endoscopies = new List<Endoscopy>();

        _logger.LogInformation("DevDataSeeder: starting seeding...");

        // ⬇️ Disable versioning for the entire seeding pass
        _db.DisableVersioning = true;

        string[] firstNames =
        [
            "Γιάννης", "Ελένη", "Κώστας", "Μαρία", "Νίκος", "Κατερίνα",
            "Παναγιώτης", "Δημήτρης", "Αγγελική", "Χρήστος", "Σοφία"
        ];

        string[] lastNames =
        [
            "Παπαδόπουλος", "Ιωάννου", "Νικολάου", "Κωνσταντίνου",
            "Γεωργίου", "Χιωτάκος", "Σταθόπουλος", "Αναστασίου"
        ];

        string[] fathersNames =
        [
            "Κωνσταντίνος", "Γεώργιος", "Ιωάννης", "Δημήτριος",
            "Νικόλαος", "Παναγιώτης"
        ];

        var today = DateTime.UtcNow.Date;
        var patientsFrom = today.AddYears(-95);
        var patientsTo = today.AddYears(-18);
        try
        {
            //
            // PATIENTS
            //
            for (int i = 0; i < targetPatientCount; i++)
            {
                var firstName = Pick(firstNames);
                var lastName = Pick(lastNames);
                var fathersName = Pick(fathersNames);

                var personalNumber = GeneratePersonalNumber();
                var birthDay = RandomDate(patientsFrom, patientsTo);

                var gender = _random.Next(3) switch
                {
                    0 => Gender.Male,
                    1 => Gender.Female,
                    _ => Gender.Other
                };

                var patient = new Patient
                {
                    FirstName = firstName,
                    LastName = lastName,
                    FathersName = fathersName,
                    PersonalNumber = personalNumber,
                    BirthDay = birthDay,
                    Gender = gender,
                    Email = $"demo{i}@example.com",
                    PhoneNumber = $"69{_random.Next(10000000, 99999999)}",
                    // Only set Address if the entity really has it (it does in your code)
                    Address = "Δημοκρατίας 1, Αθήνα"
                };

                patients.Add(patient);
            }

            await _db.Patients.AddRangeAsync(patients, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            //
            // APPOINTMENTS & VISITS
            //
            var visitsFrom = today.AddYears(-5);
            var visitsTo = today;

            foreach (var patient in patients)
            {
                int appCount = _random.Next(minAppointmentsPerPatient, maxAppointmentsPerPatient + 1);
                int visitCount = _random.Next(minVisitsPerPatient, maxVisitsPerPatient + 1);

                // Appointments (some may be in the near future)
                for (int j = 0; j < appCount; j++)
                {
                    var start = RandomDateTime(visitsFrom, visitsTo.AddMonths(1));
                    var durationMinutes = _random.Next(15, 60);

                    var appointment = new Appointment
                    {
                        PatientId = patient.Id,
                        Purpose = (AppointmentPurpose)_random.Next(1, 6), // skip Undefined = 0
                        StartDateTimeUtc = start,
                        EndDateTimeUtc = start.AddMinutes(durationMinutes),
                        Canceled = false,
                        Urgent = _random.NextDouble() < 0.1,
                        TookPlace = _random.NextDouble() < 0.8,
                        PlannedEndoscopyType = _random.NextDouble() < 0.3
                            ? (EndoscopyType?)RandomEndoscopyType()
                            : null,
                        PreparationProtocolId = null
                    };

                    appointments.Add(appointment);
                }

                // Visits
                for (int j = 0; j < visitCount; j++)
                {
                    var date = RandomDateTime(visitsFrom, visitsTo);

                    var visit = new Visit
                    {
                        PatientId = patient.Id,
                        DateOfVisitUtc = date,
                        Notes = "Demo visit generated by DevDataSeeder."
                    };

                    visits.Add(visit);
                }
            }

            await _db.Appointments.AddRangeAsync(appointments, cancellationToken);
            await _db.Visits.AddRangeAsync(visits, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            //
            // Link some visits ↔ appointments (so not everything is completely random)
            //
            foreach (var patient in patients)
            {
                var patientVisits = visits
                    .Where(v => v.PatientId == patient.Id)
                    .OrderBy(v => v.DateOfVisitUtc)
                    .ToList();

                var patientAppointments = appointments
                    .Where(a => a.PatientId == patient.Id)
                    .OrderBy(a => a.StartDateTimeUtc)
                    .ToList();

                var pairCount = Math.Min(patientVisits.Count, patientAppointments.Count);
                pairCount = Math.Min(pairCount, 10); // don’t over-link

                for (int k = 0; k < pairCount; k++)
                {
                    var visit = patientVisits[k];
                    var app = patientAppointments[k];

                    // Visit has AppointmentId FK
                    visit.AppointmentId = app.Id;
                }
            }

            await _db.SaveChangesAsync(cancellationToken);

            //
            // ENDOSCOPIES (2–3 per patient)
            //
            foreach (var patient in patients)
            {
                var patientVisits = visits
                    .Where(v => v.PatientId == patient.Id)
                    .OrderBy(v => v.DateOfVisitUtc)
                    .ToList();

                if (patientVisits.Count == 0)
                    continue;

                int endoCount = _random.Next(minEndoscopiesPerPatient, maxEndoscopiesPerPatient + 1);
                endoCount = Math.Min(endoCount, patientVisits.Count);

                for (int j = 0; j < endoCount; j++)
                {
                    var visit = patientVisits[_random.Next(patientVisits.Count)];
                    var when = visit.DateOfVisitUtc.AddHours(_random.Next(1, 6));

                    var endo = new Endoscopy
                    {
                        PatientId = patient.Id,
                        VisitId = visit.Id,
                        Type = RandomEndoscopyType(),
                        PerformedAtUtc = when,
                        IsUrgent = _random.NextDouble() < 0.05,
                        Notes = "Demo endoscopy generated by DevDataSeeder."
                    };

                    endoscopies.Add(endo);
                }
            }

            await _db.Endoscopies.AddRangeAsync(endoscopies, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }
        finally
        {
            // ⬆️ Re-enable versioning
            _db.DisableVersioning = false;
        }
        _logger.LogInformation(
            "DevDataSeeder: Seeded {Patients} patients, {Appointments} appointments, {Visits} visits, {Endoscopies} endoscopies.",
            patients.Count, appointments.Count, visits.Count, endoscopies.Count);
    }

    private DateTime RandomDate(DateTime fromInclusive, DateTime toInclusive)
    {
        if (toInclusive <= fromInclusive)
            return fromInclusive.Date;

        var range = (toInclusive - fromInclusive).Days;
        var offset = _random.Next(0, range + 1);

        return fromInclusive.AddDays(offset).Date;
    }

    private DateTime RandomDateTime(DateTime fromInclusive, DateTime toExclusive)
    {
        if (toExclusive <= fromInclusive)
            return fromInclusive;

        var rangeMinutes = (toExclusive - fromInclusive).TotalMinutes;
        var offset = _random.NextDouble() * rangeMinutes;

        return fromInclusive.AddMinutes(offset);
    }

    private PersonalNumber GeneratePersonalNumber()
    {
        // PersonalNumber requires exactly 12 digits
        var chars = new char[12];
        for (int i = 0; i < chars.Length; i++)
        {
            chars[i] = (char)('0' + _random.Next(0, 10));
        }

        var raw = new string(chars);

        // This will succeed because we just generated 12 digits
        return PersonalNumber.Create(raw);
    }


    private EndoscopyType RandomEndoscopyType()
    {
        var values = new[]
        {
            EndoscopyType.Gastroscopy,
            EndoscopyType.Colonoscopy,
            EndoscopyType.Sigmoidoscopy,
            EndoscopyType.ERCP,
            EndoscopyType.EUS
        };

        return Pick(values);
    }

    private T Pick<T>(IReadOnlyList<T> items)
    {
        if (items.Count == 0)
            throw new InvalidOperationException("Cannot pick from empty list.");

        var index = _random.Next(0, items.Count);
        return items[index];
    }
}
