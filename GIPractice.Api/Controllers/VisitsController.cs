using GIPractice.Api.Models;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VisitsController(AppDbContext db) : ControllerBase
{
    private readonly AppDbContext _db = db;

    // GET /api/Visits/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<VisitDto>> Get(int id)
    {
        var v = await _db.Visits
            .Include(v => v.Patient)
            .Include(v => v.Appointment)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (v == null)
            return NotFound();

        var dto = new VisitDto
        {
            Id = v.Id,
            AppointmentId = v.AppointmentId,
            PatientId = v.PatientId,
            PatientFullName = v.Patient.FirstName + " " + v.Patient.LastName,
            DateOfVisitUtc = v.DateOfVisitUtc,
            Notes = v.Notes
        };

        return Ok(dto);
    }
    // GET /api/visits/{id}/details
    [HttpGet("{id:int}/details")]
    public async Task<ActionResult<VisitDetailsDto>> GetDetails(int id)
    {
        // Load the visit with patient, appointment and diagnoses
        var visit = await _db.Visits
            .AsNoTracking()
            .Include(v => v.Patient)
            .Include(v => v.Appointment)
                .ThenInclude(a => a!.PreparationProtocol)
            .Include(v => v.Diagnoses)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (visit == null)
            return NotFound();

        // 1) Map Visit -> VisitDto (same shape as the simple GET)
        var visitDto = new VisitDto
        {
            Id = visit.Id,
            AppointmentId = visit.AppointmentId,
            PatientId = visit.PatientId,
            PatientFullName = visit.Patient.FirstName + " " + visit.Patient.LastName,
            DateOfVisitUtc = visit.DateOfVisitUtc,
            Notes = visit.Notes
        };

        // 2) Map Appointment -> AppointmentDto (if present)
        AppointmentDto? appointmentDto = null;
        if (visit.Appointment != null)
        {
            var a = visit.Appointment;

            appointmentDto = new AppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                // We know the patient is the same as visit.Patient
                PatientFullName = visit.Patient.FirstName + " " + visit.Patient.LastName,
                Purpose = a.Purpose,
                StartDateTimeUtc = a.StartDateTimeUtc,
                EndDateTimeUtc = a.EndDateTimeUtc,
                Canceled = a.Canceled,
                Urgent = a.Urgent,
                TookPlace = a.TookPlace,
                PlannedEndoscopyType = a.PlannedEndoscopyType,
                PreparationProtocolId = a.PreparationProtocolId,
                PreparationProtocolName = a.PreparationProtocol?.Name,
                Notes = a.Notes
            };
        }

        // 3) Endoscopies for this visit -> EndoscopyListItemDto
        var endoscopies = await _db.Endoscopies
            .AsNoTracking()
            .Include(e => e.Patient)
            .Include(e => e.BiopsyBottles)
            .Include(e => e.Report)
            .Where(e => e.VisitId == visit.Id)
            .OrderByDescending(e => e.PerformedAtUtc)
            .ThenBy(e => e.Id)
            .ToListAsync();

        var endoscopyDtos = endoscopies
            .Select(e => new EndoscopyListItemDto
            {
                Id = e.Id,
                PatientId = e.PatientId,
                PatientFullName = e.Patient.FirstName + " " + e.Patient.LastName,
                VisitId = e.VisitId,
                Type = e.Type,
                PerformedAtUtc = e.PerformedAtUtc,
                IsUrgent = e.IsUrgent,
                Notes = e.Notes,
                BiopsyBottlesCount = e.BiopsyBottles.Count,
                HasReport = e.Report != null
            })
            .ToList();

        // 4) Diagnoses attached to this visit -> DiagnosisDto
        var diagnosisDtos = visit.Diagnoses
            .Select(d => new DiagnosisDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                Standard = d.Standard
            })
            .OrderBy(d => d.Code)
            .ThenBy(d => d.Name)
            .ToList();

        // 5) Compose VisitDetailsDto
        var result = new VisitDetailsDto
        {
            Visit = visitDto,
            Appointment = appointmentDto,
            Endoscopies = endoscopyDtos,
            Diagnoses = diagnosisDtos
        };

        return Ok(result);
    }

    // POST /api/Visits

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] VisitCreateDto dto)
    {
        int patientId;
        int? appointmentId = null;

        // CASE 1: Visit linked to an appointment
        if (dto.AppointmentId.HasValue)
        {
            var appt = await _db.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == dto.AppointmentId.Value);

            if (appt == null)
            {
                ModelState.AddModelError(nameof(dto.AppointmentId), "Appointment does not exist.");
                return ValidationProblem(ModelState);
            }

            appointmentId = appt.Id;
            patientId = appt.PatientId;

            appt.TookPlace = true;
            appt.EndDateTimeUtc ??= dto.DateOfVisitUtc ?? DateTime.UtcNow;
        }
        // CASE 2: Walk-in visit (no appointment)
        else
        {
            if (!dto.PatientId.HasValue)
            {
                ModelState.AddModelError(nameof(dto.PatientId),
                    "PatientId is required when AppointmentId is null.");
                return ValidationProblem(ModelState);
            }

            var patientExists = await _db.Patients
                .AnyAsync(p => p.Id == dto.PatientId.Value);

            if (!patientExists)
            {
                ModelState.AddModelError(nameof(dto.PatientId), "Patient does not exist.");
                return ValidationProblem(ModelState);
            }

            patientId = dto.PatientId.Value;
        }

        var date = dto.DateOfVisitUtc ?? DateTime.UtcNow;

        var visit = new Visit
        {
            AppointmentId = appointmentId,   // <-- null for walk-in, real id otherwise
            PatientId = patientId,
            DateOfVisitUtc = date,
            Notes = dto.Notes
        };

        _db.Visits.Add(visit);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = visit.Id }, null);
    }
    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<VisitListItemDto>>> GetList(
    [FromQuery] int? patientId = null,
    [FromQuery] DateTime? fromUtc = null,
    [FromQuery] DateTime? toUtc = null)
    {
        var query = _db.Visits
            .AsNoTracking()
            .Include(v => v.Patient)
            .AsQueryable();

        if (patientId.HasValue)
            query = query.Where(v => v.PatientId == patientId.Value);

        if (fromUtc.HasValue)
            query = query.Where(v => v.DateOfVisitUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(v => v.DateOfVisitUtc <= toUtc.Value);

        var visits = await query
            .OrderByDescending(v => v.DateOfVisitUtc)
            .ThenBy(v => v.Id)
            .ToListAsync();

        var dto = visits
        .Select(v => new VisitListItemDto
        {
            Id = v.Id,
            PatientId = v.PatientId,
            PatientFullName = v.Patient.FirstName + " " + v.Patient.LastName,
            DateOfVisitUtc = v.DateOfVisitUtc,
            Notes = v.Notes
        })
        .ToList();
        return Ok(dto);
    }
}
