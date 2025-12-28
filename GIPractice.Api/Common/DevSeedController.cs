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

        // Idempotent seed: if any patients exist, do nothing
        if (await _db.Patients.AnyAsync(ct))
            return Ok(new { seeded = false, reason = "Patients already exist." });

        PersonalNumber.TryCreate("000000000000", out var pn);

        var p = new Patient
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

        var v = new Visit
        {
            PatientId = p.Id,
            DateOfVisitUtc = DateTime.UtcNow.AddDays(-1),
            Notes = "Seed visit",
            AppointmentId = null
        };
        _db.Visits.Add(v);
        await _db.SaveChangesAsync(ct);

        var e = new Endoscopy
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

        var b = new BiopsyBottle
        {
            PatientId = p.Id,
            EndoscopyId = e.Id,
            CollectedAtUtc = e.PerformedAtUtc,
            Label = "A1",
            Number = 1
        };
        _db.BiopsyBottles.Add(b);
        await _db.SaveChangesAsync(ct);

        return Ok(new { seeded = true, patientId = p.Id, visitId = v.Id, endoscopyId = e.Id, biopsyBottleId = b.Id });
    }
}
