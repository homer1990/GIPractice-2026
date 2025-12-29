using FluentAssertions;
using GIPractice.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GIPractice.Api.Tests;

public sealed class MigrationsSmokeTests
{
    [Fact]
    public void MigrateFreshDatabase_ShouldSucceed_WhenOptedIn()
    {
        // This is intentionally opt-in because migrations might not be the current focus
        // while contracts/controllers are still moving.
        var optIn = Environment.GetEnvironmentVariable("RUN_MIGRATIONS_TESTS");
        if (!string.Equals(optIn, "1", StringComparison.Ordinal))
            throw new Xunit.Sdk.SkipException("Set RUN_MIGRATIONS_TESTS=1 to run migration smoke test.");

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
