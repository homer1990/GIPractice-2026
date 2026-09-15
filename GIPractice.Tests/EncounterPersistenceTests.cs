using GIPractice.Core.Entities;
using GIPractice.Core.Enums;
using GIPractice.Core.ValueObjects;
using GIPractice.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Tests;

[TestClass]
public sealed class EncounterPersistenceTests
{
    [TestMethod]
    public async Task Encounter_can_exist_without_appointment_and_soft_delete_hides_details()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();

        var patient = new Patient
        {
            FirstName = "Test",
            LastName = "Patient",
            FathersName = "Father",
            PersonalNumber = PersonalNumber.Create("123456789012"),
            BirthDay = new DateTime(1980, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Gender = Gender.Male
        };

        var encounter = new Encounter
        {
            Kind = EncounterKind.Endoscopy,
            StartedAtUtc = DateTime.UtcNow,
            Patient = patient
        };

        var endoscopy = new Endoscopy
        {
            Encounter = encounter,
            Type = EndoscopyType.Colonoscopy
        };
        encounter.Endoscopy = endoscopy;

        db.Encounters.Add(encounter);
        await db.SaveChangesAsync();

        Assert.IsTrue(encounter.Id > 0);
        Assert.IsTrue(endoscopy.Id > 0);
        Assert.IsNull(encounter.AppointmentId);
        Assert.AreEqual(encounter.Id, endoscopy.EncounterId);

        var creationVersions = await db.VersionHistories
            .AsNoTracking()
            .Where(v => v.EntityName == nameof(Encounter) && v.EntityId == encounter.Id)
            .ToListAsync();
        Assert.IsTrue(creationVersions.Any(v => v.ChangeType == VersionChangeType.Created));

        db.Encounters.Remove(encounter);
        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();

        Assert.IsFalse(await db.Encounters.AnyAsync(e => e.Id == encounter.Id));
        Assert.IsFalse(await db.Endoscopies.AnyAsync(e => e.Id == endoscopy.Id));

        var history = await db.VersionHistories
            .AsNoTracking()
            .Where(v => v.EntityName == nameof(Encounter) && v.EntityId == encounter.Id)
            .OrderBy(v => v.Version)
            .ToListAsync();

        Assert.AreEqual(2, history.Count);
        Assert.AreEqual(VersionChangeType.Created, history[0].ChangeType);
        Assert.AreEqual(VersionChangeType.Deleted, history[1].ChangeType);
    }
}
