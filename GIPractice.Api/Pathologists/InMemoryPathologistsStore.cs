using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathologists;
using GIPractice.Contracts.Pathology;

namespace GIPractice.Api.Pathologists;

public sealed class InMemoryPathologistsStore : IPathologistsStore
{
    private readonly object _lock = new();
    private int _nextId = 1;
    private long _rv = DateTime.UtcNow.Ticks;

    private sealed class Row
    {
        public required PathologistId Id { get; init; }
        public string Name { get; set; } = "";
        public string? Address { get; set; }
        public string Email { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public string PricingPlanJson { get; set; } = "{}";
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }

    private readonly Dictionary<int, Row> _rows = new();

    public Task<ResultDto<PagedResultDto<PathologistListItemDto>>> SearchAsync(PathologistSearchRequestDto request, CancellationToken ct = default)
    {
        Row[] snapshot;
        lock (_lock) snapshot = _rows.Values.ToArray();

        var q = snapshot.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var s = request.Name.Trim();
            q = q.Where(x =>
                x.Name.Contains(s, StringComparison.OrdinalIgnoreCase) ||
                x.Email.Contains(s, StringComparison.OrdinalIgnoreCase));
        }

        var paging = request.Paging ?? new PagedRequestDto(1, 50);
        var total = q.Count();

        var items = q
            .OrderBy(x => x.Name)
            .Skip((paging.Page - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .Select(x => new PathologistListItemDto(
                Id: x.Id,
                Name: x.Name,
                Email: x.Email,
                PhoneNumber: x.PhoneNumber))
            .ToList();

        return Task.FromResult(ResultDto<PagedResultDto<PathologistListItemDto>>.Ok(
            new PagedResultDto<PathologistListItemDto>(items, total, paging.Page, paging.PageSize)));
    }

    public Task<ResultDto<PathologistDto>> GetAsync(PathologistId id, CancellationToken ct = default)
    {
        lock (_lock)
        {
            if (!_rows.TryGetValue(id.Value, out var row))
                return Task.FromResult(ResultDto<PathologistDto>.Fail(ErrorCodes.NotFound, "Pathologist not found."));

            return Task.FromResult(ResultDto<PathologistDto>.Ok(ToDto(row)));
        }
    }

    public Task<ResultDto<PathologistId>> CreateAsync(PathologistUpsertRequestDto request, CancellationToken ct = default)
    {
        lock (_lock)
        {
            var id = new PathologistId(_nextId++);
            var row = new Row
            {
                Id = id,
                Name = request.Name,
                Address = request.Address,
                Email = request.Email ?? string.Empty,
                PhoneNumber = request.PhoneNumber,
                PricingPlanJson = request.PricingPlanJson ?? "{}",
                RowVersion = NextRv()
            };

            _rows[id.Value] = row;
            return Task.FromResult(ResultDto<PathologistId>.Ok(id));
        }
    }

    public Task<ResultDto<bool>> UpdateAsync(PathologistUpsertRequestDto request, CancellationToken ct = default)
    {
        if (request.Id is null)
            return Task.FromResult(ResultDto<bool>.Fail(ErrorCodes.Validation, "Id is required for update."));

        lock (_lock)
        {
            if (!_rows.TryGetValue(request.Id.Value.Value, out var row))
                return Task.FromResult(ResultDto<bool>.Fail(ErrorCodes.NotFound, "Pathologist not found."));

            if (request.RowVersion is not null && !request.RowVersion.SequenceEqual(row.RowVersion))
                return Task.FromResult(ResultDto<bool>.Fail(ErrorCodes.Conflict, "RowVersion conflict."));

            row.Name = request.Name;
            row.Address = request.Address;
            row.Email = request.Email ?? string.Empty;
            row.PhoneNumber = request.PhoneNumber;
            row.PricingPlanJson = request.PricingPlanJson ?? "{}";
            row.RowVersion = NextRv();

            return Task.FromResult(ResultDto<bool>.Ok(true));
        }
    }

    public Task<ResultDto<bool>> DeleteAsync(PathologistId id, CancellationToken ct = default)
    {
        lock (_lock)
        {
            var removed = _rows.Remove(id.Value);
            return Task.FromResult(removed
                ? ResultDto<bool>.Ok(true)
                : ResultDto<bool>.Fail(ErrorCodes.NotFound, "Pathologist not found."));
        }
    }

    private PathologistDto ToDto(Row row) => new(
        Id: row.Id,
        Name: row.Name,
        Address: row.Address,
        Email: row.Email,
        PhoneNumber: row.PhoneNumber,
        PricingPlanJson: row.PricingPlanJson,
        RowVersion: row.RowVersion);

    private byte[] NextRv()
    {
        var v = Interlocked.Increment(ref _rv);
        return BitConverter.GetBytes(v);
    }
}
