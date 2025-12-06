using GIPractice.Api.Models;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController(AppDbContext db) : ControllerBase
{
    private readonly AppDbContext _db = db;

    /// <summary>
    /// Global schedule endpoint.
    /// 
    /// Example:
    ///   GET /api/schedule?fromUtc=2025-01-01T00:00:00Z&toUtc=2025-01-08T00:00:00Z
    ///   GET /api/schedule?fromUtc=2025-01-01T00:00:00Z&toUtc=2025-01-02T00:00:00Z&patientId=123
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ScheduleRangeDto>> Get(
        [FromQuery] DateTime fromUtc,
        [FromQuery] DateTime toUtc,
        [FromQuery] int? patientId = null)
    {
        if (fromUtc == default || toUtc == default)
            return BadRequest("fromUtc and toUtc are required.");

        if (fromUtc >= toUtc)
            return BadRequest("fromUtc must be earlier than toUtc.");

        //
        // 1) Appointments in range
        //
        var apptQuery = _db.Appointments
            .AsNoTracking()
            .Include(a => a.Patient)
            .AsQueryable();

        if (patientId.HasValue)
            apptQuery = apptQuery.Where(a => a.PatientId == patientId.Value);

        // Simple "starts in range" filter – good enough for day/week views.
        apptQuery = apptQuery.Where(a =>
            a.StartDateTimeUtc >= fromUtc &&
            a.StartDateTimeUtc < toUtc);

        var appointments = await apptQuery.ToListAsync();

        //
        // 2) Visits in range (including walk-ins without Appointment)
        //
        var visitQuery = _db.Visits
            .AsNoTracking()
            .Include(v => v.Patient)
            .AsQueryable();

        if (patientId.HasValue)
            visitQuery = visitQuery.Where(v => v.PatientId == patientId.Value);

        visitQuery = visitQuery.Where(v =>
            v.DateOfVisitUtc >= fromUtc &&
            v.DateOfVisitUtc < toUtc);

        var visits = await visitQuery.ToListAsync();

        //
        // 3) Project to DTOs
        //
        var items = new List<ScheduleItemDto>();

        // Appointments
        foreach (var a in appointments)
        {
            if (a.StartDateTimeUtc == default) continue;

            items.Add(new ScheduleItemDto
            {
                Type = ScheduleItemType.Appointment,
                EntityId = a.Id,
                PatientId = a.PatientId,
                PatientFullName = a.Patient.FirstName + " " + a.Patient.LastName,
                StartUtc = a.StartDateTimeUtc,
                EndUtc = a.EndDateTimeUtc,
                Canceled = a.Canceled,
                TookPlace = a.TookPlace,
                Urgent = a.Urgent,
                AppointmentPurpose = a.Purpose,
                PlannedEndoscopyType = a.PlannedEndoscopyType,
                Notes = a.Notes
            });
        }

        // Visits
        foreach (var v in visits)
        {
            if (v.DateOfVisitUtc == default) continue;

            items.Add(new ScheduleItemDto
            {
                Type = ScheduleItemType.Visit,
                EntityId = v.Id,
                PatientId = v.PatientId,
                PatientFullName = v.Patient.FirstName + " " + v.Patient.LastName,
                StartUtc = v.DateOfVisitUtc,
                EndUtc = null,
                Canceled = null,
                TookPlace = null,
                Urgent = null,
                AppointmentPurpose = null,
                PlannedEndoscopyType = null,
                Notes = v.Notes
            });
        }

        // 4) Order by time, then type, then patient name – stable and deterministic
        items = [.. items
            .OrderBy(i => i.StartUtc)
            .ThenBy(i => i.Type)
            .ThenBy(i => i.PatientFullName)];

        var result = new ScheduleRangeDto
        {
            FromUtc = fromUtc,
            ToUtc = toUtc,
            Items = items
        };

        return Ok(result);
    }
}
