using GIPractice.Api.Models;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/patients/{patientId:int}/visits")]
public class PatientVisitsController(AppDbContext db) : ControllerBase
{
    private readonly AppDbContext _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VisitDto>>> GetForPatient(int patientId)
    {
        // Make sure the patient exists
        var exists = await _db.Patients
            .AsNoTracking()
            .AnyAsync(p => p.Id == patientId);

        if (!exists)
            return NotFound("Patient not found.");

        // Load visits with patient + appointment info (if any)
        var visits = await _db.Visits
            .Include(v => v.Patient)
            .Include(v => v.Appointment)
            .AsNoTracking()
            .Where(v => v.PatientId == patientId)
            .OrderByDescending(v => v.DateOfVisitUtc)
            .ThenBy(v => v.Id)
            .ToListAsync();

        // Map to VisitDto (same shape as VisitsController.Get)
        var dto = visits
            .Select(v => new VisitDto
            {
                Id = v.Id,
                AppointmentId = v.AppointmentId,
                PatientId = v.PatientId,
                PatientFullName = v.Patient.FirstName + " " + v.Patient.LastName,
                DateOfVisitUtc = v.DateOfVisitUtc,
                Notes = v.Notes
            })
            .ToList();

        return Ok(dto);
    }
}
