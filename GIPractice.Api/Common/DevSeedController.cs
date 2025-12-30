using GIPractice.Core.Entities;
using GIPractice.Core.Enums;
using GIPractice.Core.ValueObjects;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Common;

[ApiController]
[Route("api/dev/seed")]
public sealed class DevSeedController : ControllerBase
{
    private readonly AppDbContext _db;
    public DevSeedController(AppDbContext db) => _db = db;

    [HttpPost]
    public async Task<IActionResult> Seed(CancellationToken ct)
    {
        if (!HttpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
            return NotFound();

        // Idempotent seed:
        // - Ensure there is at least 1 patient
        // - Ensure that patient has at least 1 visit
        // - Ensure that visit has at least 1 endoscopy
        // - Ensure that endoscopy has at least 1 biopsy bottle
        // This makes tests/dev UX stable even if the DB already has partial data.

        // Patient
        var p = await _db.Patients
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(ct);

        if (p is null)
        {
            PersonalNumber.TryCreate("000000000000", out var pn);

            p = new Patient
            {
                FirstName = "Test",
                LastName = "Patient",
                FathersName = "Demo",
                PersonalNumber = pn,
                BirthDay = new DateTime(1990, 1, 1),
                Gender = Gender.Male,
                Email = "test@example.com",
                PhoneNumber = "6900000000",
                Address = "Athens"
            };

            _db.Patients.Add(p);
            await _db.SaveChangesAsync(ct);
        }

        // Visit
        var v = await _db.Visits
            .Where(x => x.PatientId == p.Id)
            .OrderByDescending(x => x.DateOfVisitUtc)
            .FirstOrDefaultAsync(ct);

        if (v is null)
        {
            v = new Visit
            {
                PatientId = p.Id,
                DateOfVisitUtc = DateTime.UtcNow.AddDays(-1),
                Notes = "Seed visit",
                AppointmentId = null
            };
            _db.Visits.Add(v);
            await _db.SaveChangesAsync(ct);
        }

        // Endoscopy
        var e = await _db.Endoscopies
            .Where(x => x.PatientId == p.Id && x.VisitId == v.Id)
            .OrderByDescending(x => x.PerformedAtUtc)
            .FirstOrDefaultAsync(ct);

        if (e is null)
        {
            e = new Endoscopy
            {
                PatientId = p.Id,
                VisitId = v.Id,
                Type = EndoscopyType.Gastroscopy,
                PerformedAtUtc = DateTime.UtcNow.AddDays(-1),
                IsUrgent = false,
                Notes = "Seed endoscopy"
            };
            _db.Endoscopies.Add(e);
            await _db.SaveChangesAsync(ct);
        }

        // Biopsy bottle
        var b = await _db.BiopsyBottles
            .Include(x => x.OrganAreas)
            .Where(x => x.PatientId == p.Id && x.EndoscopyId == e.Id)
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(ct);

        if (b is null)
        {
            b = new BiopsyBottle
            {
                PatientId = p.Id,
                EndoscopyId = e.Id,
                CollectedAtUtc = e.PerformedAtUtc,
                Label = "A1",
                Number = 1
            };

            // Make SiteDescription non-empty via OrganAreas (Contracts require it).
            var oa = await _db.OrganAreas.FirstOrDefaultAsync(x => x.Code == "GEJ", ct);
            if (oa is not null)
                b.OrganAreas.Add(oa);

            _db.BiopsyBottles.Add(b);
            await _db.SaveChangesAsync(ct);
        }
        else if (b.OrganAreas.Count == 0)
        {
            // If a bottle exists but has no areas, attach one so API contracts/tests stay valid.
            var oa = await _db.OrganAreas.FirstOrDefaultAsync(x => x.Code == "GEJ", ct);
            if (oa is not null)
            {
                b.OrganAreas.Add(oa);
                await _db.SaveChangesAsync(ct);
            }
        }

        return Ok(new { seeded = true, patientId = p.Id, visitId = v.Id, endoscopyId = e.Id, biopsyBottleId = b.Id });
    }
}
