using FluentAssertions;
using GIPractice.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GIPractice.Api.Tests;

public sealed class MigrationsSmokeTests
{
    [Fact(Skip = "Enable when you want to validate migrations against a fresh database.")]
    public void MigrateFreshDatabase_ShouldSucceed_WhenOptedIn()
    {
        // This is intentionally opt-in because migrations might not be the current focus
        // while contracts/controllers are still moving.
        var optIn = Environment.GetEnvironmentVariable("RUN_MIGRATIONS_TESTS");

        var dbName = $"GIPractice_Migrate_Test_{Guid.NewGuid():N}";
        var cs =
            $"Server=(localdb)\\MSSQLLocalDB;Database={dbName};Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True";

        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(cs)
            .Options;

        using var db = new AppDbContext(opts);

        db.Database.EnsureDeleted();
        db.Database.Migrate();

        // if we got here, it worked
        db.Database.CanConnect().Should().BeTrue();

        db.Database.EnsureDeleted();
    }
}
