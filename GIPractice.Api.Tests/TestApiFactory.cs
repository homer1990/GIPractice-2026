using System.Collections.Generic;
using System.Linq;
using GIPractice.Contracts.Auth;
using GIPractice.Core.Entities.Identity;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace GIPractice.Api.Tests;

public sealed class TestApiFactory : WebApplicationFactory<ApiEntryPoint>
{
    private string? _dbName;

    public HttpClient CreateAuthenticatedClient(WebApplicationFactoryClientOptions? options = null)
    {
        var client = base.CreateClient(options ?? new WebApplicationFactoryClientOptions());
        AuthTestHelper.AuthenticateAsAdminAsync(client).GetAwaiter().GetResult();
        return client;
    }

    public HttpClient CreateAnonymousClient(WebApplicationFactoryClientOptions? options = null)
        => base.CreateClient(options ?? new WebApplicationFactoryClientOptions());

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        // CRITICAL: disable bootstrap hosted service for tests (it hits DB during startup)
        builder.ConfigureAppConfiguration((_, cfg) =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["BootstrapAdmin:Enabled"] = "false"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Replace DbContext with per-test-db
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
        // Let WebApplicationFactory build/start normally (no builder.Build()/host.Start() here)
        var host = base.CreateHost(builder);

        // Now create DB + seed data (bootstrap is disabled so startup didn't touch DB)
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        SeedTestData(db);
        SeedIdentity(scope.ServiceProvider);

        return host;
    }

    private static void SeedTestData(AppDbContext db)
    {
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

    private static void SeedIdentity(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        foreach (var role in AppRoles.All)
        {
            if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
            {
                var r = roleManager.CreateAsync(new ApplicationRole { Name = role }).GetAwaiter().GetResult();
                if (!r.Succeeded)
                    throw new InvalidOperationException("Role seed failed: " +
                                                        string.Join("; ", r.Errors.Select(e => e.Description)));
            }
        }

        var admin = userManager.FindByNameAsync("admin").GetAwaiter().GetResult();
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = "admin",
                DisplayName = "Administrator",
                IsActive = true
            };

            var created = userManager.CreateAsync(admin, "admin").GetAwaiter().GetResult();
            if (!created.Succeeded)
                throw new InvalidOperationException("Admin seed failed: " +
                                                    string.Join("; ", created.Errors.Select(e => e.Description)));
        }

        if (!userManager.IsInRoleAsync(admin, AppRoles.Admin).GetAwaiter().GetResult())
        {
            var added = userManager.AddToRoleAsync(admin, AppRoles.Admin).GetAwaiter().GetResult();
            if (!added.Succeeded)
                throw new InvalidOperationException("Admin role seed failed: " +
                                                    string.Join("; ", added.Errors.Select(e => e.Description)));
        }
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
