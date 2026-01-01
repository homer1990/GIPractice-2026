using GIPractice.Core.Entities;
using GIPractice.Core.Enums;
using GIPractice.Core.ValueObjects;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Common;

[ApiController]
[Route("api/dev")]
[AllowAnonymous]
public sealed class DevSeedController(AppDbContext db, IWebHostEnvironment env) : ControllerBase
{
    [HttpPost("seed")]
    public async Task<IActionResult> Seed(CancellationToken ct)
    {
        if (!env.IsDevelopment())
            return NotFound();

        db.DisableVersioning = true;

        // Patient
        var patient = await db.Patients
            .OrderBy(p => p.Id)
            .FirstOrDefaultAsync(ct);

        if (patient is null)
        {
            patient = new Patient
            {
                FirstName = "Seed",
                LastName = "Patient",
                FathersName = "Seeder",
                BirthDay = new DateTime(1980, 1, 1),
                Gender = Gender.Male,
                PersonalNumber = PersonalNumber.Create("000000000001"),
                PhoneNumber = null,
                Email = null
            };

            db.Patients.Add(patient);
            await db.SaveChangesAsync(ct);
        }

        // Visit
        var visit = await db.Visits
            .Where(v => v.PatientId == patient.Id)
            .OrderBy(v => v.Id)
            .FirstOrDefaultAsync(ct);

        if (visit is null)
        {
            visit = new Visit
            {
                PatientId = patient.Id,
                DateOfVisitUtc = DateTime.UtcNow.Date,
                Notes = "Seed visit"
            };

            db.Visits.Add(visit);
            await db.SaveChangesAsync(ct);
        }

        // Endoscopy
        var endoscopy = await db.Endoscopies
            .Where(e => e.PatientId == patient.Id)
            .OrderBy(e => e.Id)
            .FirstOrDefaultAsync(ct);

        if (endoscopy is null)
        {
            endoscopy = new Endoscopy
            {
                PatientId = patient.Id,
                VisitId = visit.Id,
                Type = EndoscopyType.Gastroscopy,
                PerformedAtUtc = DateTime.UtcNow,
                IsUrgent = false,
                Notes = "Seed endoscopy"
            };

            db.Endoscopies.Add(endoscopy);
            await db.SaveChangesAsync(ct);
        }

        // Biopsy bottle
        var bottle = await db.BiopsyBottles
            .Where(b => b.PatientId == patient.Id && b.EndoscopyId == endoscopy.Id)
            .OrderBy(b => b.Id)
            .FirstOrDefaultAsync(ct);

        if (bottle is null)
        {
            bottle = new BiopsyBottle
            {
                PatientId = patient.Id,
                EndoscopyId = endoscopy.Id,
                Label = "A",
                Number = 1
            };

            // Best-effort: attach first organ area if any exist.
            var oa = await db.OrganAreas
                .OrderBy(x => x.Id)
                .FirstOrDefaultAsync(ct);

            if (oa is not null)
                bottle.OrganAreas.Add(oa);

            db.BiopsyBottles.Add(bottle);
            await db.SaveChangesAsync(ct);
        }

        return Ok(new
        {
            PatientId = patient.Id,
            VisitId = visit.Id,
            EndoscopyId = endoscopy.Id,
            BiopsyBottleId = bottle.Id
        });
    }
}
