using System.Linq.Expressions;
using System.Text.Json;
using GIPractice.Core.Abstractions;
using GIPractice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using GIPractice.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GIPractice.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{
    public bool DisableVersioning { get; set; }
    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Visit> Visits => Set<Visit>();
    public DbSet<Endoscopy> Endoscopies => Set<Endoscopy>();
    public DbSet<BiopsyBottle> BiopsyBottles => Set<BiopsyBottle>();
    public DbSet<OrganArea> OrganAreas => Set<OrganArea>();
    public DbSet<Finding> Findings => Set<Finding>();
    public DbSet<Observation> Observations => Set<Observation>();
    public DbSet<MediaFile> MediaFiles => Set<MediaFile>();
    public DbSet<EndoscopyMedia> EndoscopyMedias => Set<EndoscopyMedia>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<PreparationProtocol> PreparationProtocols => Set<PreparationProtocol>();
    public DbSet<VersionHistory> VersionHistories => Set<VersionHistory>();
    public DbSet<Doctor> Doctors => Set<Doctor>();
    public DbSet<Lab> Labs => Set<Lab>();
    public DbSet<ActiveSubstance> ActiveSubstances => Set<ActiveSubstance>();
    public DbSet<Medicine> Medicines => Set<Medicine>();
    public DbSet<Diagnosis> Diagnoses => Set<Diagnosis>();
    public DbSet<Test> Tests => Set<Test>();
    public DbSet<Treatment> Treatments => Set<Treatment>();
    public DbSet<Operation> Operations => Set<Operation>();
    public DbSet<InfaiTest> InfaiTests => Set<InfaiTest>();
    public DbSet<Organ> Organs => Set<Organ>();
    public DbSet<OrganAreaOrgan> OrganAreaOrgans => Set<OrganAreaOrgan>();
    public DbSet<LocalizationString> LocalizationStrings => Set<LocalizationString>();
    public DbSet<FieldName> FieldNames => Set<FieldName>();
    public DbSet<Localization> Localizations => Set<Localization>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Prevent multiple cascade path errors: disable cascade delete for relationships
        // that target MediaFile or Endoscopy (these produce multiple cascade paths in SQL Server)
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var foreignKey in entityType.GetForeignKeys())
            {
                var principalClr = foreignKey.PrincipalEntityType.ClrType;
                if (typeof(MediaFile).IsAssignableFrom(principalClr) || principalClr == typeof(Endoscopy))
                {
                    foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
                }
            }
        }

        // Global soft-delete filter for all ISoftDelete entities
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;

            // Only apply to CLR types that implement ISoftDelete
            if (!typeof(ISoftDelete).IsAssignableFrom(clrType))
                continue;

            // Skip derived types and owned types - only root entity types can have a query filter
            if (entityType.BaseType != null || entityType.IsOwned())
                continue;

            // EXCLUDE taxonomy/reference tables from soft-delete
            if (clrType == typeof(Organ) || clrType == typeof(OrganArea))
                continue;

            var parameter = Expression.Parameter(clrType, "e");
            var prop = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
            var compare = Expression.Equal(prop, Expression.Constant(false));
            var lambda = Expression.Lambda(compare, parameter);
            builder.Entity(clrType).HasQueryFilter(lambda);
        }
    }

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        // Always keep audit info (if you want seed rows to have CreatedAt/By)
        AddAuditInfo();

        // But allow seeder to turn OFF version history
        if (!DisableVersioning)
        {
            await AddVersionsAsync(cancellationToken);
        }

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void AddAuditInfo()
    {
        var now = DateTime.UtcNow;
        const string systemUser = "system";

        var entries = ChangeTracker
            .Entries<IAuditable>()
            .ToList();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = now;
                entry.Entity.CreatedBy ??= systemUser;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = now;
                entry.Entity.UpdatedBy = systemUser;
            }
        }
    }

    private async Task AddVersionsAsync(CancellationToken cancellationToken)
    {
        var tracked = ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.State is EntityState.Added
                               or EntityState.Modified
                               or EntityState.Deleted)
            .ToList();

        if (tracked.Count == 0)
            return;

        var now = DateTime.UtcNow;
        const string systemUser = "system";

        foreach (var entry in tracked)
        {
            var entity = entry.Entity;
            var entityName = entity.GetType().Name;
            var entityId = entity.Id;

            var snapshot = JsonSerializer.Serialize(entity, entity.GetType());

            var currentMax = await VersionHistories
                .Where(v => v.EntityName == entityName && v.EntityId == entityId)
                .Select(v => (int?)v.Version)
                .MaxAsync(cancellationToken);

            var nextVersion = (currentMax ?? 0) + 1;

            var changeType = entry.State switch
            {
                EntityState.Added => VersionChangeType.Created,
                EntityState.Modified => VersionChangeType.Updated,
                EntityState.Deleted => VersionChangeType.Deleted,
                _ => VersionChangeType.Updated
            };

            var createdBy = entity.UpdatedBy ?? entity.CreatedBy ?? systemUser;

            var vh = new VersionHistory
            {
                EntityName = entityName,
                EntityId = entityId,
                Version = nextVersion,
                ChangeType = changeType,
                SnapshotJson = snapshot,
                CreatedAtUtc = now,
                CreatedBy = createdBy
            };

            await VersionHistories.AddAsync(vh, cancellationToken);

            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entity.IsDeleted = true;
            }
        }
    }
}
