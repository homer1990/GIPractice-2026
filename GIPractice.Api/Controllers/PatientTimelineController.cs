// GIPractice.Api/Controllers/PatientTimelineController.cs

using GIPractice.Api.Models;
using GIPractice.Core.Enums;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Controllers;

[ApiController]
[Route("api/patients/{patientId:int}/timeline")]
public class PatientTimelineController(AppDbContext db) : ControllerBase
{
    private readonly AppDbContext _db = db;

    // GET /api/patients/{patientId}/timeline?fromUtc=2025-01-01T00:00:00Z&toUtc=2025-12-31T23:59:59Z
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientTimelineItemDto>>> GetTimeline(
        int patientId,
        [FromQuery] DateTime? fromUtc = null,
        [FromQuery] DateTime? toUtc = null)
    {
        // Ensure patient exists
        var patientExists = await _db.Patients
            .AsNoTracking()
            .AnyAsync(p => p.Id == patientId);

        if (!patientExists)
            return NotFound("Patient not found.");

        var items = new List<PatientTimelineItemDto>();

        // 1) Appointments
        var appointments = await _db.Appointments
            .AsNoTracking()
            .Where(a => a.PatientId == patientId)
            .ToListAsync();

        foreach (var a in appointments)
        {
            if (a.StartDateTimeUtc == default) continue;

            items.Add(new PatientTimelineItemDto
            {
                WhenUtc = a.StartDateTimeUtc,
                Kind = "Appointment",
                SourceId = a.Id,
                Title = $"Appointment - {a.Purpose}",
                Description = a.Notes,
                Extra = a.Urgent ? "Urgent" : null
            });
        }

        // 2) Visits
        var visits = await _db.Visits
            .AsNoTracking()
            .Where(v => v.PatientId == patientId)
            .ToListAsync();

        foreach (var v in visits)
        {
            if (v.DateOfVisitUtc == default) continue;

            items.Add(new PatientTimelineItemDto
            {
                WhenUtc = v.DateOfVisitUtc,
                Kind = "Visit",
                SourceId = v.Id,
                Title = "Visit",
                Description = v.Notes
            });
        }

        // 3) Endoscopies
        var endoscopies = await _db.Endoscopies
            .AsNoTracking()
            .Where(e => e.PatientId == patientId)
            .ToListAsync();

        foreach (var e in endoscopies)
        {
            if (e.PerformedAtUtc == default) continue;

            items.Add(new PatientTimelineItemDto
            {
                WhenUtc = e.PerformedAtUtc,
                Kind = "Endoscopy",
                SourceId = e.Id,
                Title = $"Endoscopy - {e.Type}",
                Description = e.Notes,
                Extra = e.IsUrgent ? "Urgent" : null
            });
        }

        // 4) Lab Tests
        var tests = await _db.Tests
            .AsNoTracking()
            .Where(t => t.PatientId == patientId)
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

            items.Add(new PatientTimelineItemDto
            {
                WhenUtc = t.PerformedAtUtc,
                Kind = "Test",
                SourceId = t.Id,
                Title = "Lab Test",
                Description = t.Notes,
                Extra = extraParts.Count > 0 ? string.Join("; ", extraParts) : null
            });
        }

        // 5) Infai Tests
        var infaiTests = await _db.InfaiTests
            .AsNoTracking()
            .Where(i => i.PatientId == patientId)
            .ToListAsync();

        foreach (var i in infaiTests)
        {
            if (i.PerformedAtUtc == default) continue;

            items.Add(new PatientTimelineItemDto
            {
                WhenUtc = i.PerformedAtUtc,
                Kind = "InfaiTest",
                SourceId = i.Id,
                Title = "Infai Breath Test",
                Description = i.Notes,
                Extra = $"Result: {i.Result}"
            });
        }

        // 6) Operations
        var operations = await _db.Operations
            .AsNoTracking()
            .Where(o => o.PatientId == patientId)
            .ToListAsync();

        foreach (var o in operations)
        {
            if (o.DateAndTimeUtc == default) continue;

            items.Add(new PatientTimelineItemDto
            {
                WhenUtc = o.DateAndTimeUtc,
                Kind = "Operation",
                SourceId = o.Id,
                Title = $"Operation - {o.Type}",
                Description = null,
                Extra = o.Outcome != Outcomes.None ? $"Outcome: {o.Outcome}" : null
            });
        }

        // 7) Treatments
        var treatments = await _db.Treatments
            .AsNoTracking()
            .Where(t => t.PatientId == patientId)
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

            items.Add(new PatientTimelineItemDto
            {
                WhenUtc = t.StartUtc,
                Kind = "Treatment",
                SourceId = t.Id,
                Title = "Treatment",
                Description = t.Notes,
                Extra = extraParts.Count > 0 ? string.Join("; ", extraParts) : null
            });
        }

        // Filter by date range (if provided)
        if (fromUtc.HasValue)
            items = [.. items.Where(i => i.WhenUtc >= fromUtc.Value)];

        if (toUtc.HasValue)
            items = [.. items.Where(i => i.WhenUtc <= toUtc.Value)];

        // Sort descending (most recent first)
        items = [.. items
            .OrderByDescending(i => i.WhenUtc)
            .ThenBy(i => i.Kind)];

        return Ok(items);
    }
}
