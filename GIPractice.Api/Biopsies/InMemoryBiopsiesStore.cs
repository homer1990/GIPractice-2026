using GIPractice.Contracts.Biopsies;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;

namespace GIPractice.Api.Biopsies;

public sealed class InMemoryBiopsiesStore : IBiopsiesStore
{
    private readonly object _lock = new();

    private int _nextBottleId = 1;
    private int _nextBundleId = 1;

    private sealed class BottleRow
    {
        public BiopsyBottleId Id { get; init; }
        public PatientId PatientId { get; set; }
        public EndoscopyId EndoscopyId { get; set; }
        public string LabelCode { get; set; } = "";
        public string[] OrganAreaCodes { get; set; } = Array.Empty<string>();
        public bool IsUrgent { get; set; }
        public string? Notes { get; set; }
        public byte[] RowVersion { get; set; } = NewRowVersion();
    }

    private sealed class BundleRow
    {
        public BiopsyDispatchBundleId Id { get; init; }
        public DateTime CreatedUtc { get; set; }
        public string ProtocolNumber { get; set; } = "";
        public string? Notes { get; set; }
        public bool IsClosed { get; set; }
        public byte[] RowVersion { get; set; } = NewRowVersion();
    }

    private readonly List<BottleRow> _bottles = new();
    private readonly List<BundleRow> _bundles = new();

    public Task<PagedResultDto<BiopsyBottleDto>> SearchBottlesAsync(BiopsyBottleSearchRequestDto request, CancellationToken ct)
    {
        lock (_lock)
        {
            IEnumerable<BottleRow> q = _bottles;

            if (request.PatientId is { } pid)
                q = q.Where(x => x.PatientId.Equals(pid));

            if (request.EndoscopyId is { } eid)
                q = q.Where(x => x.EndoscopyId.Equals(eid));

            if (request.IsUrgent is { } urg)
                q = q.Where(x => x.IsUrgent == urg);

            q = q.OrderByDescending(x => x.Id.Value);

            var page = request.Paging?.Page ?? 1;
            var pageSize = request.Paging?.PageSize ?? 50;
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;

            var total = q.Count();
            var items = q.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(x => new BiopsyBottleDto(
                    x.Id,
                    x.PatientId,
                    x.EndoscopyId,
                    LabelCode: x.LabelCode,
                    SiteDescription: x.OrganAreaCodes is { Length: > 0 } ? string.Join(", ", x.OrganAreaCodes) : "",
                    IsUrgent: x.IsUrgent,
                    Notes: x.Notes,
                    RowVersion: x.RowVersion))
                .ToList();

            return Task.FromResult(new PagedResultDto<BiopsyBottleDto>(items, total, page, pageSize));
        }
    }

    public Task<BiopsyBottleDto?> GetBottleAsync(BiopsyBottleId id, CancellationToken ct)
    {
        lock (_lock)
        {
            var x = _bottles.FirstOrDefault(b => b.Id.Equals(id));
            if (x is null) return Task.FromResult<BiopsyBottleDto?>(null);

            return Task.FromResult<BiopsyBottleDto?>(new BiopsyBottleDto(
                x.Id,
                x.PatientId,
                x.EndoscopyId,
                LabelCode: x.LabelCode,
                SiteDescription: x.OrganAreaCodes is { Length: > 0 } ? string.Join(", ", x.OrganAreaCodes) : "",
                IsUrgent: x.IsUrgent,
                Notes: x.Notes,
                RowVersion: x.RowVersion));
        }
    }

    public Task<ResultDto<BiopsyBottleId>> CreateBottleAsync(BiopsyBottleUpsertRequestDto request, CancellationToken ct)
    {
        lock (_lock)
        {
            var id = new BiopsyBottleId(_nextBottleId++);

            var row = new BottleRow
            {
                Id = id,
                PatientId = request.PatientId,
                EndoscopyId = request.EndoscopyId,
                LabelCode = request.LabelCode,
                OrganAreaCodes = request.OrganAreaCodes ?? Array.Empty<string>(),
                IsUrgent = request.IsUrgent,
                Notes = request.Notes,
                RowVersion = NewRowVersion()
            };

            _bottles.Add(row);
            return Task.FromResult(ResultDto<BiopsyBottleId>.Ok(id));
        }
    }

    public Task<ResultDto<bool>> UpdateBottleAsync(BiopsyBottleUpsertRequestDto request, CancellationToken ct)
    {
        if (request.Id is null)
            return Task.FromResult(ResultDto<bool>.Fail("invalid", "Id is required for update."));

        lock (_lock)
        {
            var row = _bottles.FirstOrDefault(b => b.Id.Equals(request.Id.Value));
            if (row is null)
                return Task.FromResult(ResultDto<bool>.Fail("not_found", "Biopsy bottle not found."));

            if (request.RowVersion is not null && !row.RowVersion.SequenceEqual(request.RowVersion))
                return Task.FromResult(ResultDto<bool>.Fail("conflict", "RowVersion conflict."));

            row.PatientId = request.PatientId;
            row.EndoscopyId = request.EndoscopyId;
            row.LabelCode = request.LabelCode;
            row.IsUrgent = request.IsUrgent;
            row.Notes = request.Notes;

            if (request.OrganAreaCodes is not null)
                row.OrganAreaCodes = request.OrganAreaCodes;

            row.RowVersion = NewRowVersion();

            return Task.FromResult(ResultDto<bool>.Ok(true));
        }
    }

    public Task<ResultDto<bool>> DeleteBottleAsync(BiopsyBottleId id, CancellationToken ct)
    {
        lock (_lock)
        {
            var idx = _bottles.FindIndex(b => b.Id.Equals(id));
            if (idx < 0)
                return Task.FromResult(ResultDto<bool>.Fail("not_found", "Biopsy bottle not found."));

            _bottles.RemoveAt(idx);
            return Task.FromResult(ResultDto<bool>.Ok(true));
        }
    }

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

            // We don't actually model bundle rows yet (will be connected to Endoscopies later).
            var bundleDto = new BiopsyDispatchBundleDto(
                b.Id, b.CreatedUtc, b.ProtocolNumber, b.Notes, b.IsClosed, b.RowVersion);

            var rows = Array.Empty<BiopsyDispatchRowDto>();

            return Task.FromResult<BiopsyDispatchDetailsDto?>(new BiopsyDispatchDetailsDto(bundleDto, rows));
        }
    }

    public Task<ResultDto<BiopsyDispatchBundleId>> CreateDispatchBundleAsync(BiopsyDispatchCreateRequestDto request, CancellationToken ct)
    {
        lock (_lock)
        {
            var id = new BiopsyDispatchBundleId(_nextBundleId++);

            var row = new BundleRow
            {
                Id = id,
                CreatedUtc = DateTime.UtcNow,
                ProtocolNumber = request.ProtocolNumber,
                Notes = request.Notes,
                IsClosed = false,
                RowVersion = NewRowVersion()
            };

            _bundles.Add(row);
            return Task.FromResult(ResultDto<BiopsyDispatchBundleId>.Ok(id));
        }
    }

    public Task<ResultDto<bool>> CloseDispatchBundleAsync(BiopsyDispatchCloseRequestDto request, CancellationToken ct)
    {
        lock (_lock)
        {
            if (request.Id is null)
                return Task.FromResult(ResultDto<bool>.Fail("validation", "Id is required."));

            var row = _bundles.FirstOrDefault(x => x.Id.Equals(request.Id.Value));
            if (row is null)
                return Task.FromResult(ResultDto<bool>.Fail("not_found", "Dispatch bundle not found."));

            if (request.RowVersion is not null && !row.RowVersion.SequenceEqual(request.RowVersion))
                return Task.FromResult(ResultDto<bool>.Fail("conflict", "RowVersion conflict."));

            row.IsClosed = true;
            row.RowVersion = NewRowVersion();

            return Task.FromResult(ResultDto<bool>.Ok(true));
        }
    }

    private static byte[] NewRowVersion()
        => BitConverter.GetBytes(DateTime.UtcNow.Ticks);
}
