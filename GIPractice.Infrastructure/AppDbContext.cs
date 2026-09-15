using System.Linq.Expressions;
using System.Text.Json;
using GIPractice.Core.Abstractions;
using GIPractice.Core.Entities;
using GIPractice.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

        var customSoftDeleteFilters = new HashSet<Type>
        {
            typeof(Visit),
            typeof(Endoscopy),
            typeof(Exam),
            typeof(InfaiTest),
            typeof(BiopsyBottle),
            typeof(Observation),
            typeof(Report)
        };

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (!typeof(ISoftDelete).IsAssignableFrom(clrType) ||
                entityType.BaseType != null ||
                entityType.IsOwned() ||
                customSoftDeleteFilters.Contains(clrType))
            {
                continue;
            }

            // Organ and OrganArea are reference taxonomy. Their historical rows must
            // remain resolvable even when no longer offered for new data entry.
            if (clrType == typeof(Organ) || clrType == typeof(OrganArea))
                continue;

            var parameter = Expression.Parameter(clrType, "entity");
            var isDeleted = Expression.Property(parameter, nameof(ISoftDelete.IsDeleted));
            var predicate = Expression.Equal(isDeleted, Expression.Constant(false));
            builder.Entity(clrType).HasQueryFilter(Expression.Lambda(predicate, parameter));
        }

        // A soft-deleted Encounter hides its detail row and endoscopy-owned clinical
        // records even if those rows themselves were never individually soft-deleted.
        builder.Entity<Visit>()
            .HasQueryFilter(x => !x.IsDeleted && !x.Encounter.IsDeleted);
        builder.Entity<Endoscopy>()
            .HasQueryFilter(x => !x.IsDeleted && !x.Encounter.IsDeleted);
        builder.Entity<Exam>()
            .HasQueryFilter(x => !x.IsDeleted && !x.Encounter.IsDeleted);
        builder.Entity<InfaiTest>()
            .HasQueryFilter(x => !x.IsDeleted && !x.Encounter.IsDeleted);
        builder.Entity<BiopsyBottle>()
            .HasQueryFilter(x => !x.IsDeleted && !x.Endoscopy.IsDeleted && !x.Endoscopy.Encounter.IsDeleted);
        builder.Entity<Observation>()
            .HasQueryFilter(x => !x.IsDeleted && !x.Endoscopy.IsDeleted && !x.Endoscopy.Encounter.IsDeleted);
        builder.Entity<Report>()
            .HasQueryFilter(x => !x.IsDeleted && !x.Endoscopy.IsDeleted && !x.Endoscopy.Encounter.IsDeleted);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var candidates = PrepareTrackedChanges();

        if (DisableVersioning || candidates.Count == 0)
            return base.SaveChanges(acceptAllChangesOnSuccess);

        if (!acceptAllChangesOnSuccess)
        {
            throw new InvalidOperationException(
                "Versioned saves require acceptAllChangesOnSuccess=true. " +
                "Set DisableVersioning=true only for controlled maintenance operations that need false.");
        }

        using var transaction = Database.IsRelational() && Database.CurrentTransaction is null
            ? Database.BeginTransaction()
            : null;

        try
        {
            var affected = base.SaveChanges(false);
            var histories = BuildVersionHistory(candidates);

            // Generated keys now exist. Mark the domain write as accepted before the
            // second save so it is not written twice.
            ChangeTracker.AcceptAllChanges();

            if (histories.Count > 0)
            {
                VersionHistories.AddRange(histories);
                affected += base.SaveChanges(true);
            }

            transaction?.Commit();
            return affected;
        }
        catch
        {
            transaction?.Rollback();
            throw;
        }
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        SaveChangesAsync(true, cancellationToken);

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        var candidates = PrepareTrackedChanges();

        if (DisableVersioning || candidates.Count == 0)
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        if (!acceptAllChangesOnSuccess)
        {
            throw new InvalidOperationException(
                "Versioned saves require acceptAllChangesOnSuccess=true. " +
                "Set DisableVersioning=true only for controlled maintenance operations that need false.");
        }

        await using var transaction = Database.IsRelational() && Database.CurrentTransaction is null
            ? await Database.BeginTransactionAsync(cancellationToken)
            : null;

        try
        {
            var affected = await base.SaveChangesAsync(false, cancellationToken);
            var histories = await BuildVersionHistoryAsync(candidates, cancellationToken);

            ChangeTracker.AcceptAllChanges();

            if (histories.Count > 0)
            {
                await VersionHistories.AddRangeAsync(histories, cancellationToken);
                affected += await base.SaveChangesAsync(true, cancellationToken);
            }

            if (transaction is not null)
                await transaction.CommitAsync(cancellationToken);

            return affected;
        }
        catch
        {
            if (transaction is not null)
                await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private List<VersionCandidate> PrepareTrackedChanges()
    {
        var candidates = ChangeTracker.Entries<BaseEntity>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .Select(entry => new VersionCandidate(entry, GetChangeType(entry)))
            .ToList();

        ConvertDeletesToSoftDeletes();
        AddAuditInfo();
        return candidates;
    }

    private static VersionChangeType GetChangeType(EntityEntry<BaseEntity> entry)
    {
        if (entry.State == EntityState.Added)
            return VersionChangeType.Created;

        if (entry.State == EntityState.Deleted)
            return VersionChangeType.Deleted;

        var property = entry.Property(nameof(ISoftDelete.IsDeleted));
        if (property.IsModified)
        {
            var wasDeleted = (bool)(property.OriginalValue ?? false);
            var isDeleted = (bool)(property.CurrentValue ?? false);

            if (wasDeleted && !isDeleted)
                return VersionChangeType.Restored;
            if (!wasDeleted && isDeleted)
                return VersionChangeType.Deleted;
        }

        return VersionChangeType.Updated;
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

    private List<VersionHistory> BuildVersionHistory(IReadOnlyCollection<VersionCandidate> candidates)
    {
        var keys = GetCandidateKeys(candidates);
        var existing = LoadExistingVersions(keys);
        return CreateHistoryRows(candidates, existing);
    }

    private async Task<List<VersionHistory>> BuildVersionHistoryAsync(
        IReadOnlyCollection<VersionCandidate> candidates,
        CancellationToken cancellationToken)
    {
        var keys = GetCandidateKeys(candidates);
        var existing = await LoadExistingVersionsAsync(keys, cancellationToken);
        return CreateHistoryRows(candidates, existing);
    }

    private static List<(string EntityName, int EntityId)> GetCandidateKeys(
        IEnumerable<VersionCandidate> candidates) =>
        candidates
            .Select(candidate => (candidate.Entry.Entity.GetType().Name, candidate.Entry.Entity.Id))
            .Where(key => key.Id > 0)
            .Distinct()
            .Select(key => (key.Name, key.Id))
            .ToList();

    private Dictionary<(string EntityName, int EntityId), int> LoadExistingVersions(
        IReadOnlyCollection<(string EntityName, int EntityId)> keys)
    {
        if (keys.Count == 0)
            return [];

        var names = keys.Select(key => key.EntityName).Distinct().ToArray();
        var ids = keys.Select(key => key.EntityId).Distinct().ToArray();

        return VersionHistories
            .AsNoTracking()
            .Where(v => names.Contains(v.EntityName) && ids.Contains(v.EntityId))
            .AsEnumerable()
            .GroupBy(v => (v.EntityName, v.EntityId))
            .ToDictionary(group => group.Key, group => group.Max(v => v.Version));
    }

    private async Task<Dictionary<(string EntityName, int EntityId), int>> LoadExistingVersionsAsync(
        IReadOnlyCollection<(string EntityName, int EntityId)> keys,
        CancellationToken cancellationToken)
    {
        if (keys.Count == 0)
            return [];

        var names = keys.Select(key => key.EntityName).Distinct().ToArray();
        var ids = keys.Select(key => key.EntityId).Distinct().ToArray();

        var rows = await VersionHistories
            .AsNoTracking()
            .Where(v => names.Contains(v.EntityName) && ids.Contains(v.EntityId))
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(v => (v.EntityName, v.EntityId))
            .ToDictionary(group => group.Key, group => group.Max(v => v.Version));
    }

    private static List<VersionHistory> CreateHistoryRows(
        IReadOnlyCollection<VersionCandidate> candidates,
        IReadOnlyDictionary<(string EntityName, int EntityId), int> existingVersions)
    {
        var nextVersions = new Dictionary<(string EntityName, int EntityId), int>(existingVersions);
        var now = DateTime.UtcNow;
        const string systemUser = "system";
        var rows = new List<VersionHistory>(candidates.Count);

        foreach (var candidate in candidates)
        {
            var entity = candidate.Entry.Entity;
            if (entity.Id <= 0)
                continue;

            var key = (entity.GetType().Name, entity.Id);
            var nextVersion = nextVersions.TryGetValue(key, out var currentVersion)
                ? currentVersion + 1
                : 1;
            nextVersions[key] = nextVersion;

            // Store only mapped scalar values. Serializing the entity graph can pull in
            // unrelated navigation objects and produce enormous/cyclic audit snapshots.
            var snapshot = candidate.Entry.Properties.ToDictionary(
                property => property.Metadata.Name,
                property => property.CurrentValue);

            rows.Add(new VersionHistory
            {
                EntityName = key.Item1,
                EntityId = key.Id,
                Version = nextVersion,
                ChangeType = candidate.ChangeType,
                SnapshotJson = JsonSerializer.Serialize(snapshot),
                CreatedAtUtc = now,
                CreatedBy = candidate.ChangeType == VersionChangeType.Created
                    ? entity.CreatedBy ?? systemUser
                    : entity.UpdatedBy ?? entity.CreatedBy ?? systemUser
            });
        }

        return rows;
    }

    private sealed record VersionCandidate(
        EntityEntry<BaseEntity> Entry,
        VersionChangeType ChangeType);
}
