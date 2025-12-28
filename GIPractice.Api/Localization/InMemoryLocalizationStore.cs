using GIPractice.Contracts.Common;
using GIPractice.Contracts.Localization;

namespace GIPractice.Api.Localization;

public sealed class InMemoryLocalizationStore : ILocalizationStore
{
    private readonly object _lock = new();

    private sealed class Row
    {
        public string Key { get; set; } = "";
        public string Culture { get; set; } = "";
        public string Value { get; set; } = "";
        public string? Module { get; set; }
        public string? Notes { get; set; }
        public byte[] RowVersion { get; set; } = NewRowVersion();
    }

    private readonly List<Row> _rows = new();

    public Task<PagedResultDto<TranslationListItemDto>> SearchAsync(TranslationSearchRequestDto request, CancellationToken ct)
    {
        lock (_lock)
        {
            IEnumerable<Row> q = _rows;

            if (!string.IsNullOrWhiteSpace(request.KeyContains))
                q = q.Where(x => x.Key.Contains(request.KeyContains, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(request.Culture))
                q = q.Where(x => string.Equals(x.Culture, request.Culture, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(request.Module))
                q = q.Where(x => string.Equals(x.Module ?? "", request.Module, StringComparison.OrdinalIgnoreCase));

            q = q.OrderBy(x => x.Culture).ThenBy(x => x.Key);

            var page = request.Paging?.Page ?? 1;
            var pageSize = request.Paging?.PageSize ?? 100;
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 100;

            var total = q.Count();
            var items = q.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(x => new TranslationListItemDto(x.Key, x.Culture, x.Module))
                .ToList();

            return Task.FromResult(new PagedResultDto<TranslationListItemDto>(items, total, page, pageSize));
        }
    }

    public Task<TranslationDto?> GetAsync(string key, string culture, CancellationToken ct)
    {
        lock (_lock)
        {
            var row = _rows.FirstOrDefault(x =>
                string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(x.Culture, culture, StringComparison.OrdinalIgnoreCase));

            if (row is null) return Task.FromResult<TranslationDto?>(null);

            return Task.FromResult<TranslationDto?>(new TranslationDto(
                row.Key, row.Culture, row.Value, row.Module, row.Notes, row.RowVersion));
        }
    }

    public Task<ResultDto<bool>> UpsertAsync(TranslationUpsertRequestDto request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Key))
            return Task.FromResult(ResultDto<bool>.Fail("validation", "Key is required."));

        if (string.IsNullOrWhiteSpace(request.Culture))
            return Task.FromResult(ResultDto<bool>.Fail("validation", "Culture is required."));

        lock (_lock)
        {
            var existing = _rows.FirstOrDefault(x =>
                string.Equals(x.Key, request.Key, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(x.Culture, request.Culture, StringComparison.OrdinalIgnoreCase));

            if (existing is null)
            {
                _rows.Add(new Row
                {
                    Key = request.Key,
                    Culture = request.Culture,
                    Value = request.Value ?? "",
                    Module = request.Module,
                    Notes = request.Notes,
                    RowVersion = NewRowVersion()
                });

                return Task.FromResult(ResultDto<bool>.Ok(true));
            }

            if (request.RowVersion is not null && !existing.RowVersion.SequenceEqual(request.RowVersion))
                return Task.FromResult(ResultDto<bool>.Fail("conflict", "RowVersion conflict."));

            existing.Value = request.Value ?? "";
            existing.Module = request.Module;
            existing.Notes = request.Notes;
            existing.RowVersion = NewRowVersion();

            return Task.FromResult(ResultDto<bool>.Ok(true));
        }
    }

    public Task<ResultDto<bool>> DeleteAsync(string key, string culture, CancellationToken ct)
    {
        lock (_lock)
        {
            var idx = _rows.FindIndex(x =>
                string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(x.Culture, culture, StringComparison.OrdinalIgnoreCase));

            if (idx < 0)
                return Task.FromResult(ResultDto<bool>.Fail("not_found", "Translation not found."));

            _rows.RemoveAt(idx);
            return Task.FromResult(ResultDto<bool>.Ok(true));
        }
    }

    private static byte[] NewRowVersion()
        => BitConverter.GetBytes(DateTime.UtcNow.Ticks);
}
