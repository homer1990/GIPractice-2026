using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using GIPractice.Core.Abstractions;
using GIPractice.Core.Entities;
using GIPractice.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{
    public bool DisableVersioning { get; set; }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Encounter> Encounters => Set<Encounter>();
    public DbSet<Visit> Visits => Set<Visit>();
    public DbSet<Endoscopy> Endoscopies => Set<Endoscopy>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<InfaiTest> InfaiTests => Set<InfaiTest>();
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
    public DbSet<Organ> Organs => Set<Organ>();
    public DbSet<OrganAreaOrgan> OrganAreaOrgans => Set<OrganAreaOrgan>();
    public DbSet<LocalizationString> LocalizationStrings => Set<LocalizationString>();
    public DbSet<FieldName> FieldNames => Set<FieldName>();
    public DbSet<Localization> Localizations => Set<Localization>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Keep cascade paths predictable on every relational provider. Aggregate roots
        // and stored media are deleted explicitly by application workflows.
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var foreignKey in entityType.GetForeignKeys())
            {
                var principalClr = foreignKey.PrincipalEntityType.ClrType;
                if (typeof(MediaFile).IsAssignableFrom(principalClr) || principalClr == typeof(Encounter))
                    foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        // Global soft-delete filter for auditable domain entities. Reference taxonomy
        // remains visible because it is effectively configuration data.
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (!typeof(ISoftDelete).IsAssignableFrom(clrType) || entityType.BaseType != null || entityType.IsOwned())
                continue;

            if (clrType == typeof(Organ) || clrType == typeof(OrganArea))
                continue;

            var parameter = Expression.Parameter(clrType, "entity");
            var isDeleted = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
            var predicate = Expression.Equal(isDeleted, Expression.Constant(false));
            builder.Entity(clrType).HasQueryFilter(Expression.Lambda(predicate, parameter));
        }
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        AddAuditInfo();
        ConvertDeletesToSoftDeletes();
        // Version history is asynchronous because it reads the current max version.
        // Sync callers still get correct audit/soft-delete semantics rather than silently
        // bypassing them as the previous implementation did.
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        SaveChangesAsync(true, cancellationToken);

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        AddAuditInfo();
        ConvertDeletesToSoftDeletes();

        if (!DisableVersioning)
            await AddVersionsAsync(cancellationToken);

        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void AddAuditInfo()
    {
        var now = DateTime.UtcNow;
        const string systemUser = "system";

        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = now;
                entry.Entity.CreatedBy ??= systemUser;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = now;
                entry.Entity.UpdatedBy ??= systemUser;
            }
        }
    }

    private void ConvertDeletesToSoftDeletes()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>().Where(e => e.State == EntityState.Deleted))
        {
            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
        }
    }

    private async Task AddVersionsAsync(CancellationToken cancellationToken)
    {
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .Where(e => e.Entity is not null)
            .ToList();

        if (entries.Count == 0)
            return;

        var now = DateTime.UtcNow;
        const string systemUser = "system";
        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        foreach (var entry in entries)
        {
            var entity = entry.Entity;
            var entityName = entity.GetType().Name;
            var entityId = entity.Id;

            // New identity-key entities do not have a durable ID until the database write.
            // Never create a misleading history row for EntityId=0.
            if (entityId == 0)
                continue;

            var currentMax = await VersionHistories
                .Where(v => v.EntityName == entityName && v.EntityId == entityId)
                .Select(v => (int?)v.Version)
                .MaxAsync(cancellationToken);

            var snapshot = JsonSerializer.Serialize(entity, entity.GetType(), jsonOptions);
            var changeType = entity.IsDeleted ? VersionChangeType.Deleted : VersionChangeType.Updated;

            await VersionHistories.AddAsync(new VersionHistory
            {
                EntityName = entityName,
                EntityId = entityId,
                Version = (currentMax ?? 0) + 1,
                ChangeType = changeType,
                SnapshotJson = snapshot,
                CreatedAtUtc = now,
                CreatedBy = entity.UpdatedBy ?? entity.CreatedBy ?? systemUser
            }, cancellationToken);
        }
    }
}
