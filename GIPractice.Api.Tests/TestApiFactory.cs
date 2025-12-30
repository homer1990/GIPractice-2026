using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace GIPractice.Api.Tests;

public sealed class TestApiFactory : WebApplicationFactory<ApiEntryPoint>
{
    private string? _dbName;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Use Development so you don't accidentally skip middleware/routes via env guards.
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            // Remove DbContext registrations + configuration delegates coming from Program.cs
            services.RemoveAll<AppDbContext>();
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<IConfigureOptions<DbContextOptions<AppDbContext>>>();
            services.RemoveAll<IDbContextOptionsConfiguration<AppDbContext>>();
            services.RemoveAll<IDbContextFactory<AppDbContext>>();

            _dbName = $"GIPractice_Test_{Guid.NewGuid():N}";
            var cs =
                $"Server=(localdb)\\MSSQLLocalDB;Database={_dbName};Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True";

            services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(cs));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // Let WebApplicationFactory build/start the host normally
        var host = base.CreateHost(builder);

        // NOW do DB init (no BuildServiceProvider inside ConfigureServices)
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        SeedTestData(db);

        return host;
    }

    private static void SeedTestData(AppDbContext db)
    {
        // Keep this minimal: seed just enough to satisfy FK constraints and basic searches.
        // (Tests may still create their own data.)

        if (db.Patients.Any())
            return;

        var p = new GIPractice.Core.Entities.Patient
        {
            LastName = "Seed",
            FirstName = "Patient",
            FathersName = "Seeder",
            PersonalNumber = GIPractice.Core.ValueObjects.PersonalNumber.Create("000000000001"),
            BirthDay = new DateTime(1980, 1, 1),
            Gender = GIPractice.Core.Enums.Gender.None,
            Email = null,
            PhoneNumber = null,
            Address = null,
        };

        db.Patients.Add(p);
        db.SaveChanges();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && _dbName is not null)
        {
            try
            {
                using var scope = Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureDeleted();
            }
            catch { /* ignore */ }
        }

        base.Dispose(disposing);
    }
}
