using GIPractice.Core.Entities;
using GIPractice.Core.Enums;
using GIPractice.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace GIPractice.Api.Tests;

public static class PathologyTestSeed
{
    /// <summary>
    /// Creates:
    /// - 1 Pathologist
    /// - 1 Visit (for an existing seeded patient)
    /// - 2 Endoscopies under that visit
    /// - 1 BiopsyBottle per endoscopy (required for parcel assignment)
    /// Sets Endoscopy.BiopsiesCost for monetary sum tests.
    /// </summary>
    public static async Task<(int PathologistId, int Endo1Id, int Endo2Id, decimal Cost1, decimal Cost2)>
        SeedPathologistAndTwoEndoscopiesAsync(
            TestApiFactory factory,
            decimal cost1,
            decimal cost2,
            bool urgent1 = false,
            bool urgent2 = false)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // TestApiFactory seeds at least one patient.
        var patientId = db.Patients.OrderBy(p => p.Id).Select(p => p.Id).First();

        // Create a fresh visit
        var visit = new Visit
        {
            PatientId = patientId,
            DateOfVisitUtc = DateTime.UtcNow.Date,
            Notes = "pathology test visit"
        };
        db.Visits.Add(visit);
        await db.SaveChangesAsync();

        // Create two endoscopies
        var endo1 = new Endoscopy
        {
            PatientId = patientId,
            VisitId = visit.Id,
            Type = EndoscopyType.Gastroscopy,
            PerformedAtUtc = DateTime.UtcNow,
            IsUrgent = urgent1,
            Notes = "pathology seed endo 1",
            BiopsiesCost = cost1
        };

        var endo2 = new Endoscopy
        {
            PatientId = patientId,
            VisitId = visit.Id,
            Type = EndoscopyType.Gastroscopy,
            PerformedAtUtc = DateTime.UtcNow.AddMinutes(5),
            IsUrgent = urgent2,
            Notes = "pathology seed endo 2",
            BiopsiesCost = cost2
        };

        db.Endoscopies.AddRange(endo1, endo2);
        await db.SaveChangesAsync();

        // Parcel logic requires biopsy bottles to exist
        db.BiopsyBottles.AddRange(
            new BiopsyBottle
            {
                PatientId = patientId,
                EndoscopyId = endo1.Id,
                CollectedAtUtc = endo1.PerformedAtUtc,
                Label = "A",
                Number = 1
            },
            new BiopsyBottle
            {
                PatientId = patientId,
                EndoscopyId = endo2.Id,
                CollectedAtUtc = endo2.PerformedAtUtc,
                Label = "B",
                Number = 2
            });

        // Create a pathologist
        var pathologist = new Pathologist
        {
            Name = "Test Pathologist",
            PricingPlanJson = "{}"
        };
        db.Pathologists.Add(pathologist);

        await db.SaveChangesAsync();

        return (pathologist.Id, endo1.Id, endo2.Id, cost1, cost2);
    }
}
