using GIPractice.Contracts.Common;
using GIPractice.Contracts.Settings;

namespace GIPractice.Api.Settings;

public sealed class InMemorySettingsStore : ISettingsStore
{
    private readonly object _lock = new();

    private sealed class Row
    {
        public string Key { get; set; } = "";
        public string Value { get; set; } = "";
        public AppSettingScope Scope { get; set; }
        public string? ScopeKey { get; set; }
        public string? Description { get; set; }
        public byte[] RowVersion { get; set; } = NewRowVersion();
    }

    private readonly List<Row> _rows = new();

    public Task<PagedResultDto<AppSettingListItemDto>> SearchAsync(AppSettingSearchRequestDto request, CancellationToken ct)
    {
        lock (_lock)
        {
            IEnumerable<Row> q = _rows;

            if (!string.IsNullOrWhiteSpace(request.KeyContains))
                q = q.Where(x => x.Key.Contains(request.KeyContains, StringComparison.OrdinalIgnoreCase));

            if (request.Scope is { } sc)
                q = q.Where(x => x.Scope == sc);

            if (!string.IsNullOrWhiteSpace(request.ScopeKey))
                q = q.Where(x => string.Equals(x.ScopeKey, request.ScopeKey, StringComparison.OrdinalIgnoreCase));

            q = q.OrderBy(x => x.Key);

            var page = request.Paging?.Page ?? 1;
            var pageSize = request.Paging?.PageSize ?? 50;
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;

            var total = q.Count();
            var items = q.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(x => new AppSettingListItemDto(x.Key, x.Scope, x.ScopeKey, x.Description))
                .ToList();

            return Task.FromResult(new PagedResultDto<AppSettingListItemDto>(items, total, page, pageSize));
        }
    }

    public Task<AppSettingDto?> GetAsync(string key, AppSettingScope scope, string? scopeKey, CancellationToken ct)
    {
        lock (_lock)
        {
            var row = _rows.FirstOrDefault(x =>
                string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase) &&
                x.Scope == scope &&
                string.Equals(x.ScopeKey ?? "", scopeKey ?? "", StringComparison.OrdinalIgnoreCase));

            if (row is null) return Task.FromResult<AppSettingDto?>(null);

            return Task.FromResult<AppSettingDto?>(new AppSettingDto(
                row.Key, row.Value, row.Scope, row.ScopeKey, row.Description, row.RowVersion));
        }
    }

    public Task<ResultDto<bool>> UpsertAsync(AppSettingUpsertRequestDto request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Key))
            return Task.FromResult(ResultDto<bool>.Fail("validation", "Key is required."));

        lock (_lock)
        {
            var existing = _rows.FirstOrDefault(x =>
                string.Equals(x.Key, request.Key, StringComparison.OrdinalIgnoreCase) &&
                x.Scope == request.Scope &&
                string.Equals(x.ScopeKey ?? "", request.ScopeKey ?? "", StringComparison.OrdinalIgnoreCase));

            if (existing is null)
            {
                _rows.Add(new Row
                {
                    Key = request.Key,
                    Value = request.Value ?? "",
                    Scope = request.Scope,
                    ScopeKey = request.ScopeKey,
                    Description = request.Description,
                    RowVersion = NewRowVersion()
                });

                return Task.FromResult(ResultDto<bool>.Ok(true));
            }

            if (request.RowVersion is not null && !existing.RowVersion.SequenceEqual(request.RowVersion))
                return Task.FromResult(ResultDto<bool>.Fail("conflict", "RowVersion conflict."));

            existing.Value = request.Value ?? "";
            existing.Description = request.Description;
            existing.RowVersion = NewRowVersion();

            return Task.FromResult(ResultDto<bool>.Ok(true));
        }
    }

    public Task<ResultDto<bool>> DeleteAsync(string key, AppSettingScope scope, string? scopeKey, CancellationToken ct)
    {
        lock (_lock)
        {
            var idx = _rows.FindIndex(x =>
                string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase) &&
                x.Scope == scope &&
                string.Equals(x.ScopeKey ?? "", scopeKey ?? "", StringComparison.OrdinalIgnoreCase));

            if (idx < 0)
                return Task.FromResult(ResultDto<bool>.Fail("not_found", "Setting not found."));

            _rows.RemoveAt(idx);
            return Task.FromResult(ResultDto<bool>.Ok(true));
        }
    }

    private static byte[] NewRowVersion()
        => BitConverter.GetBytes(DateTime.UtcNow.Ticks);
}
