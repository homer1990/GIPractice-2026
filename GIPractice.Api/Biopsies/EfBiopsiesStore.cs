using GIPractice.Contracts.Biopsies;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Core.Entities;
using GIPractice.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Biopsies;

public sealed class EfBiopsiesStore : IBiopsiesStore
{
    private readonly AppDbContext _db;

    // Temporary in-memory for Dispatch Bundles (until you add a real entity/table)
    private readonly object _lock = new();
    private int _nextBundleId = 1;

    private sealed class BundleRow
    {
        public BiopsyDispatchBundleId Id { get; init; }
        public DateTime CreatedUtc { get; set; }
        public string ProtocolNumber { get; set; } = "";
        public string? Notes { get; set; }
        public bool IsClosed { get; set; }
        public byte[] RowVersion { get; set; } = NewRowVersion();
    }

    private readonly List<BundleRow> _bundles = new();

    public EfBiopsiesStore(AppDbContext db) => _db = db;

    // ------------------------
    // Bottles (EF-backed)
    // ------------------------

    public async Task<PagedResultDto<BiopsyBottleDto>> SearchBottlesAsync(BiopsyBottleSearchRequestDto request, CancellationToken ct)
    {
        IQueryable<BiopsyBottle> q = _db.BiopsyBottles
            .AsNoTracking()
            .Include(b => b.OrganAreas)
            .Include(b => b.Endoscopy);

        if (request.PatientId is { } pid)
            q = q.Where(b => b.PatientId == pid.Value);

        if (request.EndoscopyId is { } eid)
            q = q.Where(b => b.EndoscopyId == eid.Value);

        // Bottle has no IsUrgent column. We treat "urgent bottle" as "urgent endoscopy".
        if (request.IsUrgent is { } urg)
            q = q.Where(b => b.Endoscopy.IsUrgent == urg);

        q = q.OrderByDescending(b => b.CollectedAtUtc).ThenByDescending(b => b.Id);

        var page = request.Paging?.Page ?? 1;
        var pageSize = request.Paging?.PageSize ?? 50;
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 50;

        var total = await q.CountAsync(ct);

        var rows = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = rows.Select(ToDto).ToArray();

        return new PagedResultDto<BiopsyBottleDto>(items, total, page, pageSize);
    }

    public async Task<BiopsyBottleDto?> GetBottleAsync(BiopsyBottleId id, CancellationToken ct)
    {
        var b = await _db.BiopsyBottles
            .AsNoTracking()
            .Include(x => x.OrganAreas)
            .Include(x => x.Endoscopy)
            .FirstOrDefaultAsync(x => x.Id == id.Value, ct);

        return b is null ? null : ToDto(b);
    }

    public async Task<ResultDto<BiopsyBottleId>> CreateBottleAsync(BiopsyBottleUpsertRequestDto request, CancellationToken ct)
    {
        if (request.Id is not null)
            return ResultDto<BiopsyBottleId>.Fail("invalid", "Id must be null when creating.");

        // Validate patient + endoscopy exist and match
        var endo = await _db.Endoscopies.AsNoTracking().FirstOrDefaultAsync(e => e.Id == request.EndoscopyId.Value, ct);
        if (endo is null)
            return ResultDto<BiopsyBottleId>.Fail("not_found", "Endoscopy not found.");

        if (endo.PatientId != request.PatientId.Value)
            return ResultDto<BiopsyBottleId>.Fail("validation", "Endoscopy does not belong to patient.");

        var entity = new BiopsyBottle
        {
            PatientId = request.PatientId.Value,
            EndoscopyId = request.EndoscopyId.Value,

            // Bottle has no explicit "collected" in Contracts. Use Endoscopy time as default.
            CollectedAtUtc = endo.PerformedAtUtc,

            // Contracts: LabelCode -> Entity: Label
            Label = request.LabelCode ?? "",

            // Contracts don't have Number. Keep 0 unless you later add it to Contracts.
            Number = 0
        };

        // Best-effort: interpret SiteDescription as comma-separated OrganArea *codes*
        if (!string.IsNullOrWhiteSpace(request.SiteDescription))
        {
            var codes = SplitCodes(request.SiteDescription);
            if (codes.Count > 0)
            {
                var areas = await _db.OrganAreas
                    .Where(a => codes.Contains(a.Code))
                    .ToListAsync(ct);

                entity.OrganAreas = areas;
            }
        }

        _db.BiopsyBottles.Add(entity);
        await _db.SaveChangesAsync(ct);

        return ResultDto<BiopsyBottleId>.Ok(new BiopsyBottleId(entity.Id));
    }

    public async Task<ResultDto<bool>> UpdateBottleAsync(BiopsyBottleUpsertRequestDto request, CancellationToken ct)
    {
        if (request.Id is null)
            return ResultDto<bool>.Fail("invalid", "Id is required for update.");

        var entity = await _db.BiopsyBottles
            .Include(b => b.OrganAreas)
            .Include(b => b.Endoscopy)
            .FirstOrDefaultAsync(b => b.Id == request.Id.Value.Value, ct);

        if (entity is null)
            return ResultDto<bool>.Fail("not_found", "Biopsy bottle not found.");

        if (request.RowVersion is not null && !request.RowVersion.SequenceEqual(MakeRowVersion(entity)))
            return ResultDto<bool>.Fail("conflict", "RowVersion conflict.");

        var endo = await _db.Endoscopies.AsNoTracking().FirstOrDefaultAsync(e => e.Id == request.EndoscopyId.Value, ct);
        if (endo is null)
            return ResultDto<bool>.Fail("not_found", "Endoscopy not found.");

        if (endo.PatientId != request.PatientId.Value)
            return ResultDto<bool>.Fail("validation", "Endoscopy does not belong to patient.");

        entity.PatientId = request.PatientId.Value;
        entity.EndoscopyId = request.EndoscopyId.Value;
        entity.CollectedAtUtc = endo.PerformedAtUtc;
        entity.Label = request.LabelCode ?? "";

        // Update OrganAreas from SiteDescription (codes) best-effort
        entity.OrganAreas.Clear();

        if (!string.IsNullOrWhiteSpace(request.SiteDescription))
        {
            var codes = SplitCodes(request.SiteDescription);
            if (codes.Count > 0)
            {
                var areas = await _db.OrganAreas
                    .Where(a => codes.Contains(a.Code))
                    .ToListAsync(ct);

                foreach (var a in areas)
                    entity.OrganAreas.Add(a);
            }
        }

        await _db.SaveChangesAsync(ct);
        return ResultDto<bool>.Ok(true);
    }

    public async Task<ResultDto<bool>> DeleteBottleAsync(BiopsyBottleId id, CancellationToken ct)
    {
        var entity = await _db.BiopsyBottles.FirstOrDefaultAsync(b => b.Id == id.Value, ct);
        if (entity is null)
            return ResultDto<bool>.Fail("not_found", "Biopsy bottle not found.");

        _db.BiopsyBottles.Remove(entity); // your SaveChanges hook soft-deletes
        await _db.SaveChangesAsync(ct);

        return ResultDto<bool>.Ok(true);
    }

    // ------------------------
    // Dispatch bundles (still in-memory)
    // ------------------------

    public Task<PagedResultDto<BiopsyDispatchBundleDto>> SearchDispatchBundlesAsync(BiopsyDispatchSearchRequestDto request, CancellationToken ct)
    {
        lock (_lock)
        {
            IEnumerable<BundleRow> q = _bundles;

            if (!string.IsNullOrWhiteSpace(request.ProtocolNumber))
                q = q.Where(x => x.ProtocolNumber.Contains(request.ProtocolNumber, StringComparison.OrdinalIgnoreCase));

            if (request.IsClosed is { } closed)
                q = q.Where(x => x.IsClosed == closed);

            if (request.CreatedFrom is { } df)
                q = q.Where(x => DateOnly.FromDateTime(x.CreatedUtc) >= df);

            if (request.CreatedTo is { } dt)
                q = q.Where(x => DateOnly.FromDateTime(x.CreatedUtc) <= dt);

            q = q.OrderByDescending(x => x.CreatedUtc);

            var page = request.Paging?.Page ?? 1;
            var pageSize = request.Paging?.PageSize ?? 50;
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;

            var total = q.Count();
            var items = q.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(x => new BiopsyDispatchBundleDto(
                    x.Id, x.CreatedUtc, x.ProtocolNumber, x.Notes, x.IsClosed, x.RowVersion))
                .ToList();

            return Task.FromResult(new PagedResultDto<BiopsyDispatchBundleDto>(items, total, page, pageSize));
        }
    }

    public Task<BiopsyDispatchDetailsDto?> GetDispatchDetailsAsync(BiopsyDispatchBundleId id, CancellationToken ct)
    {
        lock (_lock)
        {
            var b = _bundles.FirstOrDefault(x => x.Id.Equals(id));
            if (b is null) return Task.FromResult<BiopsyDispatchDetailsDto?>(null);

            var bundleDto = new BiopsyDispatchBundleDto(
                b.Id, b.CreatedUtc, b.ProtocolNumber, b.Notes, b.IsClosed, b.RowVersion);

            // Still not modeled as a DB relation yet:
            var rows = Array.Empty<BiopsyDispatchRowDto>();

            return Task.FromResult<BiopsyDispatchDetailsDto?>(new BiopsyDispatchDetailsDto(bundleDto, rows));
        }
    }

    public Task<ResultDto<BiopsyDispatchBundleId>> CreateDispatchBundleAsync(BiopsyDispatchCreateRequestDto request, CancellationToken ct)
    {
        lock (_lock)
        {
            if (_bundles.Any(x => string.Equals(x.ProtocolNumber, request.ProtocolNumber, StringComparison.OrdinalIgnoreCase)))
                return Task.FromResult(ResultDto<BiopsyDispatchBundleId>.Fail("conflict", "ProtocolNumber already exists."));

            var id = new BiopsyDispatchBundleId(_nextBundleId++);

            _bundles.Add(new BundleRow
            {
                Id = id,
                CreatedUtc = DateTime.UtcNow,
                ProtocolNumber = request.ProtocolNumber,
                Notes = request.Notes,
                IsClosed = false,
                RowVersion = NewRowVersion()
            });

            return Task.FromResult(ResultDto<BiopsyDispatchBundleId>.Ok(id));
        }
    }

    public Task<ResultDto<bool>> CloseDispatchBundleAsync(BiopsyDispatchCloseRequestDto request, CancellationToken ct)
    {
        lock (_lock)
        {
            var row = _bundles.FirstOrDefault(x => x.Id.Equals(request.Id));
            if (row is null)
                return Task.FromResult(ResultDto<bool>.Fail("not_found", "Dispatch bundle not found."));

            if (request.RowVersion is not null && !row.RowVersion.SequenceEqual(request.RowVersion))
                return Task.FromResult(ResultDto<bool>.Fail("conflict", "RowVersion conflict."));

            row.IsClosed = true;
            row.RowVersion = NewRowVersion();

            return Task.FromResult(ResultDto<bool>.Ok(true));
        }
    }

    // ------------------------
    // Mapping helpers
    // ------------------------

    private static BiopsyBottleDto ToDto(BiopsyBottle b)
    {
        var site = b.OrganAreas is { Count: > 0 }
            ? string.Join(", ", b.OrganAreas.Select(a => a.Code))
            : "";

        return new BiopsyBottleDto(
            new BiopsyBottleId(b.Id),
            new PatientId(b.PatientId),
            new EndoscopyId(b.EndoscopyId),
            LabelCode: b.Label,
            SiteDescription: site,
            IsUrgent: b.Endoscopy?.IsUrgent ?? false,
            Notes: b.Endoscopy?.Notes,
            RowVersion: MakeRowVersion(b));
    }

    private static HashSet<string> SplitCodes(string text)
        => text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
               .Where(s => !string.IsNullOrWhiteSpace(s))
               .ToHashSet(StringComparer.OrdinalIgnoreCase);

    private static byte[] MakeRowVersion(BiopsyBottle b)
        => BitConverter.GetBytes((b.UpdatedAtUtc ?? b.CreatedAtUtc).Ticks);

    private static byte[] NewRowVersion()
        => BitConverter.GetBytes(DateTime.UtcNow.Ticks);
}
