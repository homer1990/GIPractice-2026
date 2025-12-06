using AutoMapper;
using GIPractice.Api.Models;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/patients/{patientId:int}/appointments")]
public class PatientAppointmentsController(AppDbContext db, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _db = db;
    private readonly IMapper _mapper = mapper;

    // GET /api/patients/{patientId}/appointments
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetForPatient(int patientId)
    {
        // Ensure patient exists
        var exists = await _db.Patients
            .AsNoTracking()
            .AnyAsync(p => p.Id == patientId);

        if (!exists)
            return NotFound("Patient not found.");

        var appointments = await _db.Appointments
            .AsNoTracking()
            .Where(a => a.PatientId == patientId)
            .Include(a => a.Patient)
            .Include(a => a.PreparationProtocol)
            .OrderByDescending(a => a.StartDateTimeUtc)
            .ThenBy(a => a.Id)
            .ToListAsync();

        var dto = _mapper.Map<List<AppointmentDto>>(appointments);
        return Ok(dto);
    }
}
