using GIPractice.Api.Models;
using GIPractice.Core.Entities;
using GIPractice.Core.Enums;
using GIPractice.Core.ValueObjects;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientsController(AppDbContext db) : ControllerBase
{
    private readonly AppDbContext _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientDto>>> GetPatients()
    {
        var patients = await _db.Patients
            .OrderBy(p => p.LastName)
            .ToListAsync();

        var result = patients.Select(p => new PatientDto
        {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            FathersName = p.FathersName,
            PersonalNumber = p.PersonalNumber.Value,
            BirthDay = p.BirthDay,
            Gender = p.Gender,
            Email = p.Email,
            PhoneNumber = p.PhoneNumber,
            Address = p.Address
        });

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PatientDto>> GetPatient(int id)
    {
        var p = await _db.Patients.FindAsync(id);
        if (p == null)
            return NotFound();

        var dto = new PatientDto
        {
            Id = p.Id,
            FirstName = p.FirstName,
            LastName = p.LastName,
            FathersName = p.FathersName,
            PersonalNumber = p.PersonalNumber.Value,
            BirthDay = p.BirthDay,
            Gender = p.Gender,
            Email = p.Email,
            PhoneNumber = p.PhoneNumber,
            Address = p.Address
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult> CreatePatient([FromBody] PatientCreateDto dto)
    {
        if (!PersonalNumber.TryCreate(dto.PersonalNumber, out var pn))
        {
            ModelState.AddModelError(nameof(dto.PersonalNumber), "Invalid personal number format.");
            return ValidationProblem(ModelState);
        }

        var patient = new Patient
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            FathersName = dto.FathersName,
            PersonalNumber = pn,
            BirthDay = dto.BirthDay,
            Gender = dto.Gender,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address
        };

        _db.Patients.Add(patient);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, null);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdatePatient(int id, [FromBody] PatientCreateDto dto)
    {
        var existing = await _db.Patients.FindAsync(id);
        if (existing == null)
            return NotFound();

        if (!PersonalNumber.TryCreate(dto.PersonalNumber, out var pn))
        {
            ModelState.AddModelError(nameof(dto.PersonalNumber), "Invalid personal number format.");
            return ValidationProblem(ModelState);
        }

        existing.FirstName = dto.FirstName;
        existing.LastName = dto.LastName;
        existing.FathersName = dto.FathersName;
        existing.PersonalNumber = pn;
        existing.BirthDay = dto.BirthDay;
        existing.Gender = dto.Gender;
        existing.Email = dto.Email;
        existing.PhoneNumber = dto.PhoneNumber;
        existing.Address = dto.Address;

        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeletePatient(int id)
    {
        var p = await _db.Patients.FindAsync(id);
        if (p == null)
            return NotFound();

        p.IsDeleted = true;
        await _db.SaveChangesAsync();
        return NoContent();
    }
    // GET: api/patients/{id}/summary
    [HttpGet("{id:int}/summary")]
    public async Task<ActionResult<PatientSummaryDto>> GetSummary(int id)
    {
        var patient = await _db.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
            return NotFound();

        // Basic DTO for patient (reuse existing mapping manually)
        var patientDto = new PatientDto
        {
            Id = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            FathersName = patient.FathersName,
            PersonalNumber = patient.PersonalNumber.Value,
            BirthDay = patient.BirthDay,
            Gender = patient.Gender,
            Email = patient.Email,
            PhoneNumber = patient.PhoneNumber,
            Address = patient.Address
        };

        // Counts
        var totalVisits = await _db.Visits.CountAsync(v => v.PatientId == id);
        var totalEndoscopies = await _db.Endoscopies.CountAsync(e => e.PatientId == id);
        var totalTests = await _db.Tests.CountAsync(t => t.PatientId == id);
        var totalInfaiTests = await _db.InfaiTests.CountAsync(i => i.PatientId == id);
        var totalOperations = await _db.Operations.CountAsync(o => o.PatientId == id);
        var activeTreatments = await _db.Treatments.CountAsync(t => t.PatientId == id && t.EndUtc == null);

        // Last dates
        var lastVisitUtc = await _db.Visits
            .Where(v => v.PatientId == id)
            .OrderByDescending(v => v.DateOfVisitUtc)
            .Select(v => (DateTime?)v.DateOfVisitUtc)
            .FirstOrDefaultAsync();

        var lastEndoscopyUtc = await _db.Endoscopies
            .Where(e => e.PatientId == id)
            .OrderByDescending(e => e.PerformedAtUtc)
            .Select(e => (DateTime?)e.PerformedAtUtc)
            .FirstOrDefaultAsync();

        var lastTestUtc = await _db.Tests
            .Where(t => t.PatientId == id)
            .OrderByDescending(t => t.PerformedAtUtc)
            .Select(t => (DateTime?)t.PerformedAtUtc)
            .FirstOrDefaultAsync();

        var lastInfaiTestUtc = await _db.InfaiTests
            .Where(i => i.PatientId == id)
            .OrderByDescending(i => i.PerformedAtUtc)
            .Select(i => (DateTime?)i.PerformedAtUtc)
            .FirstOrDefaultAsync();

        var lastOperationUtc = await _db.Operations
            .Where(o => o.PatientId == id)
            .OrderByDescending(o => o.DateAndTimeUtc)
            .Select(o => (DateTime?)o.DateAndTimeUtc)
            .FirstOrDefaultAsync();

        // Diagnoses: up to 5 names
        var mainDiagnosesNames = await _db.Diagnoses
            .AsNoTracking()
            .Where(d => d.Patients.Any(p => p.Id == id))
            .OrderBy(d => d.Name)
            .Select(d => d.Name)
            .Take(5)
            .ToListAsync();

        var summary = new PatientSummaryDto
        {
            Patient = patientDto,
            TotalVisits = totalVisits,
            TotalEndoscopies = totalEndoscopies,
            TotalTests = totalTests,
            TotalInfaiTests = totalInfaiTests,
            TotalOperations = totalOperations,
            ActiveTreatments = activeTreatments,
            LastVisitUtc = lastVisitUtc,
            LastEndoscopyUtc = lastEndoscopyUtc,
            LastTestUtc = lastTestUtc,
            LastInfaiTestUtc = lastInfaiTestUtc,
            LastOperationUtc = lastOperationUtc,
            MainDiagnosesSummary = mainDiagnosesNames.Count > 0
                ? string.Join(", ", mainDiagnosesNames)
                : null
        };

        return Ok(summary);
    }
    // GET: api/patients/{id}/dashboard
    [HttpGet("{id:int}/dashboard")]
    public async Task<ActionResult<PatientDashboardDto>> GetDashboard(
        int id,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null,
        [FromQuery] int maxTimelineItems = 100)
    {
        if (maxTimelineItems <= 0)
            maxTimelineItems = 100;
        if (maxTimelineItems > 500)
            maxTimelineItems = 500;

        // 1. Load patient basic data
        var patient = await _db.Patients
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (patient == null)
            return NotFound();

        static int CalcAgeYears(DateTime birthDay)
        {
            var today = DateTime.UtcNow.Date;
            var b = birthDay.Date;
            var age = today.Year - b.Year;
            if (b > today.AddYears(-age)) age--;
            return age < 0 ? 0 : age;
        }

        // 2. Build summary (same shape as PatientSummaryController)
        var totalAppointments = await _db.Appointments.CountAsync(a => a.PatientId == id);
        var totalVisits = await _db.Visits.CountAsync(v => v.PatientId == id);
        var totalEndoscopies = await _db.Endoscopies.CountAsync(e => e.PatientId == id);
        var totalTests = await _db.Tests.CountAsync(t => t.PatientId == id);
        var totalInfaiTests = await _db.InfaiTests.CountAsync(i => i.PatientId == id);
        var totalOperations = await _db.Operations.CountAsync(o => o.PatientId == id);
        var totalTreatments = await _db.Treatments.CountAsync(t => t.PatientId == id);
        var totalDiagnoses = await _db.Diagnoses
            .CountAsync(d => d.Patients.Any(p => p.Id == id));

        var lastAppointmentUtc = await _db.Appointments
            .Where(a => a.PatientId == id)
            .OrderByDescending(a => a.StartDateTimeUtc)
            .Select(a => (DateTime?)a.StartDateTimeUtc)
            .FirstOrDefaultAsync();

        var lastVisitUtc = await _db.Visits
            .Where(v => v.PatientId == id)
            .OrderByDescending(v => v.DateOfVisitUtc)
            .Select(v => (DateTime?)v.DateOfVisitUtc)
            .FirstOrDefaultAsync();

        var lastEndoscopyUtc = await _db.Endoscopies
            .Where(e => e.PatientId == id)
            .OrderByDescending(e => e.PerformedAtUtc)
            .Select(e => (DateTime?)e.PerformedAtUtc)
            .FirstOrDefaultAsync();

        var lastTestUtc = await _db.Tests
            .Where(t => t.PatientId == id)
            .OrderByDescending(t => t.PerformedAtUtc)
            .Select(t => (DateTime?)t.PerformedAtUtc)
            .FirstOrDefaultAsync();

        var lastInfaiTestUtc = await _db.InfaiTests
            .Where(i => i.PatientId == id)
            .OrderByDescending(i => i.PerformedAtUtc)
            .Select(i => (DateTime?)i.PerformedAtUtc)
            .FirstOrDefaultAsync();

        var lastOperationUtc = await _db.Operations
            .Where(o => o.PatientId == id)
            .OrderByDescending(o => o.DateAndTimeUtc)
            .Select(o => (DateTime?)o.DateAndTimeUtc)
            .FirstOrDefaultAsync();

        var lastTreatmentStartUtc = await _db.Treatments
            .Where(t => t.PatientId == id)
            .OrderByDescending(t => t.StartUtc)
            .Select(t => (DateTime?)t.StartUtc)
            .FirstOrDefaultAsync();

        var activeTreatmentsCount = await _db.Treatments
            .CountAsync(t => t.PatientId == id && t.EndUtc == null);

        var summary = new PatientSummaryDto
        {
            PatientId = patient.Id,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            FathersName = patient.FathersName,
            PersonalNumber = patient.PersonalNumber.Value,
            BirthDay = patient.BirthDay,
            AgeYears = CalcAgeYears(patient.BirthDay),
            Email = patient.Email,
            PhoneNumber = patient.PhoneNumber,
            Address = patient.Address,

            TotalAppointments = totalAppointments,
            TotalVisits = totalVisits,
            TotalEndoscopies = totalEndoscopies,
            TotalDiagnoses = totalDiagnoses,
            TotalTests = totalTests,
            TotalInfaiTests = totalInfaiTests,
            TotalOperations = totalOperations,
            TotalTreatments = totalTreatments,

            LastAppointmentUtc = lastAppointmentUtc,
            LastVisitUtc = lastVisitUtc,
            LastEndoscopyUtc = lastEndoscopyUtc,
            LastTestUtc = lastTestUtc,
            LastInfaiTestUtc = lastInfaiTestUtc,
            LastOperationUtc = lastOperationUtc,
            LastTreatmentStartUtc = lastTreatmentStartUtc,

            ActiveTreatmentsCount = activeTreatmentsCount
        };

        // 3. Build timeline (same idea as PatientTimelineController, but limited)
        var timelineItems = new List<PatientTimelineItemDto>();

        // Appointments
        var appointments = await _db.Appointments
            .AsNoTracking()
            .Where(a => a.PatientId == id)
            .ToListAsync();

        foreach (var a in appointments)
        {
            if (a.StartDateTimeUtc == default) continue;

            timelineItems.Add(new PatientTimelineItemDto
            {
                WhenUtc = a.StartDateTimeUtc,
                Kind = "Appointment",
                SourceId = a.Id,
                Title = $"Appointment - {a.Purpose}",
                Description = a.Notes,
                Extra = a.Urgent ? "Urgent" : null
            });
        }

        // Visits
        var visits = await _db.Visits
            .AsNoTracking()
            .Where(v => v.PatientId == id)
            .ToListAsync();

        foreach (var v in visits)
        {
            if (v.DateOfVisitUtc == default) continue;

            timelineItems.Add(new PatientTimelineItemDto
            {
                WhenUtc = v.DateOfVisitUtc,
                Kind = "Visit",
                SourceId = v.Id,
                Title = "Visit",
                Description = v.Notes
            });
        }

        // Endoscopies
        var endoscopies = await _db.Endoscopies
            .AsNoTracking()
            .Where(e => e.PatientId == id)
            .ToListAsync();

        foreach (var e in endoscopies)
        {
            if (e.PerformedAtUtc == default) continue;

            timelineItems.Add(new PatientTimelineItemDto
            {
                WhenUtc = e.PerformedAtUtc,
                Kind = "Endoscopy",
                SourceId = e.Id,
                Title = $"Endoscopy - {e.Type}",
                Description = e.Notes,
                Extra = e.IsUrgent ? "Urgent" : null
            });
        }

        // Lab tests
        var tests = await _db.Tests
            .AsNoTracking()
            .Where(t => t.PatientId == id)
            .ToListAsync();

        foreach (var t in tests)
        {
            if (t.PerformedAtUtc == default) continue;

            var extraParts = new List<string>();
            if (t.TestType != TestType.None)
                extraParts.Add($"Type: {t.TestType}");
            if (t.QualitativeResult != QualitativeTestResults.None)
                extraParts.Add($"Qualitative: {t.QualitativeResult}");
            if (t.QuantitativeResult.HasValue)
                extraParts.Add($"Quantitative: {t.QuantitativeResult}");

            timelineItems.Add(new PatientTimelineItemDto
            {
                WhenUtc = t.PerformedAtUtc,
                Kind = "Test",
                SourceId = t.Id,
                Title = "Lab Test",
                Description = t.Notes,
                Extra = extraParts.Count > 0 ? string.Join("; ", extraParts) : null
            });
        }

        // Infai tests
        var infaiTests = await _db.InfaiTests
            .AsNoTracking()
            .Where(i => i.PatientId == id)
            .ToListAsync();

        foreach (var i in infaiTests)
        {
            if (i.PerformedAtUtc == default) continue;

            timelineItems.Add(new PatientTimelineItemDto
            {
                WhenUtc = i.PerformedAtUtc,
                Kind = "InfaiTest",
                SourceId = i.Id,
                Title = "Infai Breath Test",
                Description = i.Notes,
                Extra = $"Result: {i.Result}"
            });
        }

        // Operations
        var operations = await _db.Operations
            .AsNoTracking()
            .Where(o => o.PatientId == id)
            .ToListAsync();

        foreach (var o in operations)
        {
            if (o.DateAndTimeUtc == default) continue;

            timelineItems.Add(new PatientTimelineItemDto
            {
                WhenUtc = o.DateAndTimeUtc,
                Kind = "Operation",
                SourceId = o.Id,
                Title = $"Operation - {o.Type}",
                Description = null,
                Extra = o.Outcome != Outcomes.None ? $"Outcome: {o.Outcome}" : null
            });
        }

        // Treatments
        var treatments = await _db.Treatments
            .AsNoTracking()
            .Where(t => t.PatientId == id)
            .ToListAsync();

        foreach (var t in treatments)
        {
            if (t.StartUtc == default) continue;

            var extraParts = new List<string>();
            if (t.IsChronic)
                extraParts.Add("Chronic");
            if (t.EndUtc.HasValue)
                extraParts.Add($"End: {t.EndUtc.Value:yyyy-MM-dd}");
            if (t.Outcome != Outcomes.None)
                extraParts.Add($"Outcome: {t.Outcome}");

            timelineItems.Add(new PatientTimelineItemDto
            {
                WhenUtc = t.StartUtc,
                Kind = "Treatment",
                SourceId = t.Id,
                Title = "Treatment",
                Description = t.Notes,
                Extra = extraParts.Count > 0 ? string.Join("; ", extraParts) : null
            });
        }

        // 4. Apply optional date window + ordering + limit
        if (fromUtc.HasValue)
            timelineItems = [.. timelineItems.Where(i => i.WhenUtc >= fromUtc.Value)];

        if (toUtc.HasValue)
            timelineItems = [.. timelineItems.Where(i => i.WhenUtc <= toUtc.Value)];

        timelineItems = [.. timelineItems
            .OrderByDescending(i => i.WhenUtc)
            .ThenBy(i => i.Kind)
            .Take(maxTimelineItems)];

        // 5. Build dashboard DTO
        var dashboard = new PatientDashboardDto
        {
            Summary = summary,
            Timeline = timelineItems
        };

        return Ok(dashboard);
    }
}
