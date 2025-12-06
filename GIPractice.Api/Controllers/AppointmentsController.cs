using GIPractice.Api.Models;
using GIPractice.Core.Entities;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController(AppDbContext db) : ControllerBase
{
    private readonly AppDbContext _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> Get()
    {
        var appts = await _db.Appointments
            .Include(a => a.Patient)
            .Include(a => a.PreparationProtocol)
            .ToListAsync();

        var result = appts.Select(a => new AppointmentDto
        {
            Id = a.Id,
            PatientId = a.PatientId,
            PatientFullName = a.Patient.FirstName + " " + a.Patient.LastName,
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
        });

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppointmentDto>> Get(int id)
    {
        var a = await _db.Appointments
            .Include(a => a.Patient)
            .Include(a => a.PreparationProtocol)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (a == null)
            return NotFound();

        var dto = new AppointmentDto
        {
            Id = a.Id,
            PatientId = a.PatientId,
            PatientFullName = a.Patient.FirstName + " " + a.Patient.LastName,
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

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] AppointmentCreateDto dto)
    {
        // Validate patient exists
        var patientExists = await _db.Patients
            .AnyAsync(p => p.Id == dto.PatientId);
        if (!patientExists)
        {
            ModelState.AddModelError(nameof(dto.PatientId), "Patient does not exist.");
            return ValidationProblem(ModelState);
        }

        // Normalize protocol id: treat 0 as null
        if (dto.PreparationProtocolId == 0)
            dto.PreparationProtocolId = null;

        // If protocol id is specified, validate it exists
        if (dto.PreparationProtocolId.HasValue)
        {
            var protocolExists = await _db.PreparationProtocols
                .AnyAsync(p => p.Id == dto.PreparationProtocolId.Value);

            if (!protocolExists)
            {
                ModelState.AddModelError(nameof(dto.PreparationProtocolId),
                    "Preparation protocol does not exist.");
                return ValidationProblem(ModelState);
            }
        }

        var appt = new Appointment
        {
            PatientId = dto.PatientId,
            Purpose = dto.Purpose,
            StartDateTimeUtc = dto.StartDateTimeUtc,
            EndDateTimeUtc = dto.EndDateTimeUtc,
            Urgent = dto.Urgent,
            PlannedEndoscopyType = dto.PlannedEndoscopyType,
            PreparationProtocolId = dto.PreparationProtocolId,
            Notes = dto.Notes
        };

        _db.Appointments.Add(appt);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = appt.Id }, null);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, [FromBody] AppointmentCreateDto dto)
    {
        var appt = await _db.Appointments.FindAsync(id);
        if (appt == null)
            return NotFound();

        appt.PatientId = dto.PatientId;
        appt.Purpose = dto.Purpose;
        appt.StartDateTimeUtc = dto.StartDateTimeUtc;
        appt.EndDateTimeUtc = dto.EndDateTimeUtc;
        appt.Urgent = dto.Urgent;
        appt.PlannedEndoscopyType = dto.PlannedEndoscopyType;
        appt.PreparationProtocolId = dto.PreparationProtocolId;
        appt.Notes = dto.Notes;

        await _db.SaveChangesAsync();

        return NoContent();
    }
}
