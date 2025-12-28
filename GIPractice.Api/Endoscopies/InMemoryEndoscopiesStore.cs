using GIPractice.Contracts.Common;
using GIPractice.Contracts.Endoscopies;
using GIPractice.Contracts.Ids;

namespace GIPractice.Api.Endoscopies;

public sealed class InMemoryEndoscopiesStore : IEndoscopiesStore
{
    private readonly object _lock = new();
    private int _nextId = 1;

    private sealed class Row
    {
        public EndoscopyId Id { get; init; }
        public PatientId PatientId { get; set; }
        public EncounterId EncounterId { get; set; }
        public EndoscopyTypeId EndoscopyTypeId { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime? EndUtc { get; set; }
        public EndoscopyStatus Status { get; set; }
        public bool IsUrgent { get; set; }
        public string? Notes { get; set; }
        public byte[] RowVersion { get; set; } = NewRowVersion();
    }

    private readonly List<Row> _rows = new();

    public Task<PagedResultDto<EndoscopyListItemDto>> SearchAsync(EndoscopySearchRequestDto request, CancellationToken ct)
    {
        lock (_lock)
        {
            IEnumerable<Row> q = _rows;

            if (request.PatientId is { } pid)
                q = q.Where(x => x.PatientId.Equals(pid));

            if (request.EncounterId is { } encId)
                q = q.Where(x => x.EncounterId.Equals(encId));

            if (request.EndoscopyTypeId is { } et)
                q = q.Where(x => x.EndoscopyTypeId.Equals(et));

            if (request.Status is { } st)
                q = q.Where(x => x.Status == st);

            if (request.IsUrgent is { } urg)
                q = q.Where(x => x.IsUrgent == urg);

            if (request.DateFrom is { } df)
                q = q.Where(x => DateOnly.FromDateTime(x.StartUtc) >= df);

            if (request.DateTo is { } dt)
                q = q.Where(x => DateOnly.FromDateTime(x.StartUtc) <= dt);

            q = q.OrderByDescending(x => x.StartUtc);

            var page = request.Paging?.Page ?? 1;
            var pageSize = request.Paging?.PageSize ?? 50;
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;

            var total = q.Count();
            var items = q.Skip((page - 1) * pageSize).Take(pageSize)
                .Select(x => new EndoscopyListItemDto(
                    x.Id,
                    x.PatientId,
                    x.EncounterId,
                    x.EndoscopyTypeId,
                    EndoscopyTypeNameFromId(x.EndoscopyTypeId),
                    x.StartUtc,
                    x.Status,
                    x.IsUrgent))
                .ToList();

            return Task.FromResult(new PagedResultDto<EndoscopyListItemDto>(
                Items: items,
                TotalCount: total,
                Page: page,
                PageSize: pageSize));
        }
    }

    public Task<EndoscopyDetailsDto?> GetAsync(EndoscopyId id, CancellationToken ct)
    {
        lock (_lock)
        {
            var x = _rows.FirstOrDefault(r => r.Id.Equals(id));
            if (x is null) return Task.FromResult<EndoscopyDetailsDto?>(null);

            return Task.FromResult<EndoscopyDetailsDto?>(new EndoscopyDetailsDto(
                x.Id, x.PatientId, x.EncounterId, x.EndoscopyTypeId,
                x.StartUtc, x.EndUtc, x.Status, x.IsUrgent, x.Notes,
                x.RowVersion));
        }
    }

    public Task<ResultDto<EndoscopyId>> CreateAsync(EndoscopyUpsertRequestDto request, CancellationToken ct)
    {
        lock (_lock)
        {
            var id = new EndoscopyId(_nextId++);

            var row = new Row
            {
                Id = id,
                PatientId = request.PatientId,
                EncounterId = request.EncounterId,
                EndoscopyTypeId = request.EndoscopyTypeId,
                StartUtc = request.StartUtc,
                EndUtc = request.EndUtc,
                Status = request.Status,
                IsUrgent = request.IsUrgent,
                Notes = request.Notes,
                RowVersion = NewRowVersion()
            };

            _rows.Add(row);
            return Task.FromResult(ResultDto<EndoscopyId>.Ok(id));
        }
    }

    public Task<ResultDto<bool>> UpdateAsync(EndoscopyUpsertRequestDto request, CancellationToken ct)
    {
        if (request.Id is null)
            return Task.FromResult(ResultDto<bool>.Fail("invalid", "Id is required for update."));

        lock (_lock)
        {
            var row = _rows.FirstOrDefault(r => r.Id.Equals(request.Id.Value));
            if (row is null)
                return Task.FromResult(ResultDto<bool>.Fail("not_found", "Endoscopy not found."));

            if (request.RowVersion is not null && !row.RowVersion.SequenceEqual(request.RowVersion))
                return Task.FromResult(ResultDto<bool>.Fail("conflict", "RowVersion conflict."));

            row.PatientId = request.PatientId;
            row.EncounterId = request.EncounterId;
            row.EndoscopyTypeId = request.EndoscopyTypeId;
            row.StartUtc = request.StartUtc;
            row.EndUtc = request.EndUtc;
            row.Status = request.Status;
            row.IsUrgent = request.IsUrgent;
            row.Notes = request.Notes;
            row.RowVersion = NewRowVersion();

            return Task.FromResult(ResultDto<bool>.Ok(true));
        }
    }

    public Task<ResultDto<bool>> DeleteAsync(EndoscopyId id, CancellationToken ct)
    {
        lock (_lock)
        {
            var idx = _rows.FindIndex(r => r.Id.Equals(id));
            if (idx < 0)
                return Task.FromResult(ResultDto<bool>.Fail("not_found", "Endoscopy not found."));

            _rows.RemoveAt(idx);
            return Task.FromResult(ResultDto<bool>.Ok(true));
        }
    }

    private static byte[] NewRowVersion()
        => BitConverter.GetBytes(DateTime.UtcNow.Ticks);

    private static string EndoscopyTypeNameFromId(EndoscopyTypeId id)
        => id.Value switch
        {
            1 => "Gastroscopy",
            2 => "Colonoscopy",
            3 => "Sigmoidoscopy",
            _ => $"Type #{id.Value}"
        };
}
