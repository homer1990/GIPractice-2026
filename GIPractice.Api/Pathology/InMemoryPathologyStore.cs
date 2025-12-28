using GIPractice.Contracts.Common;
using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;

namespace GIPractice.Api.Pathology;

public sealed class InMemoryPathologyStore : IPathologyStore
{
    private readonly object _lock = new();
    private int _nextId = 1;

    private sealed class Row
    {
        public PathologyReportId Id { get; init; }
        public PatientId PatientId { get; set; }
        public EndoscopyId EndoscopyId { get; set; }
        public BiopsyDispatchBundleId? BiopsyDispatchBundleId { get; set; }

        public DateTime? SentUtc { get; set; }
        public DateTime? ReceivedUtc { get; set; }

        public string? ParcelId { get; set; }
        public bool IsUrgent { get; set; }

        public string? ReportText { get; set; }
        public PathologyReportStatus Status { get; set; }

        public byte[] RowVersion { get; set; } = NewRowVersion();
    }

    private readonly List<Row> _rows = new();

    public Task<PagedResultDto<PathologyReportListItemDto>> SearchAsync(PathologyReportSearchRequestDto request, CancellationToken ct)
    {
        lock (_lock)
        {
            IEnumerable<Row> q = _rows;

            if (request.PatientId is { } pid)
                q = q.Where(x => x.PatientId.Equals(pid));

            if (request.EndoscopyId is { } eid)
                q = q.Where(x => x.EndoscopyId.Equals(eid));

            if (request.BiopsyDispatchBundleId is { } bid)
                q = q.Where(x => x.BiopsyDispatchBundleId.HasValue && x.BiopsyDispatchBundleId.Value.Equals(bid));

            if (request.Status is { } st)
                q = q.Where(x => x.Status == st);

            if (request.IsUrgent is { } urg)
                q = q.Where(x => x.IsUrgent == urg);

            if (request.SentFrom is { } df)
                q = q.Where(x => x.SentUtc.HasValue && DateOnly.FromDateTime(x.SentUtc.Value) >= df);

            if (request.SentTo is { } dt)
                q = q.Where(x => x.SentUtc.HasValue && DateOnly.FromDateTime(x.SentUtc.Value) <= dt);

            q = q.OrderByDescending(x => x.Id.Value);

            var page = request.Paging?.Page ?? 1;
            var pageSize = request.Paging?.PageSize ?? 50;
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;

            var total = q.Count();
            var items = q.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(x => new PathologyReportListItemDto(
                    x.Id, x.PatientId, x.EndoscopyId, x.BiopsyDispatchBundleId,
                    x.SentUtc, x.ReceivedUtc, x.IsUrgent, x.Status))
                .ToList();

            return Task.FromResult(new PagedResultDto<PathologyReportListItemDto>(items, total, page, pageSize));
        }
    }

    public Task<PathologyReportDto?> GetAsync(PathologyReportId id, CancellationToken ct)
    {
        lock (_lock)
        {
            var x = _rows.FirstOrDefault(r => r.Id.Equals(id));
            if (x is null) return Task.FromResult<PathologyReportDto?>(null);

            return Task.FromResult<PathologyReportDto?>(new PathologyReportDto(
                x.Id, x.PatientId, x.EndoscopyId, x.BiopsyDispatchBundleId,
                x.SentUtc, x.ReceivedUtc, x.ParcelId, x.IsUrgent,
                x.ReportText, x.Status, x.RowVersion));
        }
    }

    public Task<ResultDto<PathologyReportId>> CreateAsync(PathologyReportUpsertRequestDto request, CancellationToken ct)
    {
        lock (_lock)
        {
            var id = new PathologyReportId(_nextId++);

            var row = new Row
            {
                Id = id,
                PatientId = request.PatientId,
                EndoscopyId = request.EndoscopyId,
                BiopsyDispatchBundleId = request.BiopsyDispatchBundleId,
                SentUtc = request.SentUtc,
                ReceivedUtc = request.ReceivedUtc,
                ParcelId = request.ParcelId,
                IsUrgent = request.IsUrgent,
                ReportText = request.ReportText,
                Status = request.Status,
                RowVersion = NewRowVersion()
            };

            _rows.Add(row);
            return Task.FromResult(ResultDto<PathologyReportId>.Ok(id));
        }
    }

    public Task<ResultDto<bool>> UpdateAsync(PathologyReportUpsertRequestDto request, CancellationToken ct)
    {
        if (request.Id is null)
            return Task.FromResult(ResultDto<bool>.Fail("invalid", "Id is required for update."));

        lock (_lock)
        {
            var row = _rows.FirstOrDefault(r => r.Id.Equals(request.Id.Value));
            if (row is null)
                return Task.FromResult(ResultDto<bool>.Fail("not_found", "Pathology report not found."));

            if (request.RowVersion is not null && !row.RowVersion.SequenceEqual(request.RowVersion))
                return Task.FromResult(ResultDto<bool>.Fail("conflict", "RowVersion conflict."));

            row.PatientId = request.PatientId;
            row.EndoscopyId = request.EndoscopyId;
            row.BiopsyDispatchBundleId = request.BiopsyDispatchBundleId;
            row.SentUtc = request.SentUtc;
            row.ReceivedUtc = request.ReceivedUtc;
            row.ParcelId = request.ParcelId;
            row.IsUrgent = request.IsUrgent;
            row.ReportText = request.ReportText;
            row.Status = request.Status;
            row.RowVersion = NewRowVersion();

            return Task.FromResult(ResultDto<bool>.Ok(true));
        }
    }

    public Task<ResultDto<bool>> DeleteAsync(PathologyReportId id, CancellationToken ct)
    {
        lock (_lock)
        {
            var idx = _rows.FindIndex(r => r.Id.Equals(id));
            if (idx < 0)
                return Task.FromResult(ResultDto<bool>.Fail("not_found", "Pathology report not found."));

            _rows.RemoveAt(idx);
            return Task.FromResult(ResultDto<bool>.Ok(true));
        }
    }

    private static byte[] NewRowVersion()
        => BitConverter.GetBytes(DateTime.UtcNow.Ticks);
}
