using GIPractice.Contracts.Ids;
using GIPractice.Contracts.Pathology;

namespace GIPractice.Api.Pathology;

public sealed class InMemoryPathologyRepository
{
    private readonly object _lock = new();

    private int _nextParcelId = 1;
    private int _nextReportId = 1;
    private long _rv = DateTime.UtcNow.Ticks;

    internal sealed class ParcelRow
    {
        public required PathologyParcelId Id { get; init; }
        public required PathologistId PathologistId { get; init; }
        public required string ParcelCode { get; init; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? DispatchedAtUtc { get; set; }

        public string? CourierName { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Notes { get; set; }

        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        // For fast stats; still recomputed defensively if needed
        public HashSet<int> ReportIds { get; } = new();
    }

    internal sealed class ReportRow
    {
        public required PathologyReportId Id { get; init; }

        public required PatientId PatientId { get; set; }
        public required EndoscopyId EndoscopyId { get; set; }
        public required PathologistId PathologistId { get; set; }

        public string? DispatchParcelCode { get; set; }

        public DateTime? SentAtUtc { get; set; }
        public DateTime? ReceivedAtUtc { get; set; }

        public string? Notes { get; set; }
        public string? ClinicalInfo { get; set; }
        public string? MacroscopyText { get; set; }
        public string? DiagnosisText { get; set; }

        public PathologyReportStatus Status { get; set; }
        public bool IsUrgent { get; set; }

        public MediaFileId? DocumentFileId { get; set; }
        public PathologyDocumentKind DocumentKind { get; set; }
        public string? DocumentFileName { get; set; }
        public string? DocumentContentType { get; set; }
        public PathologyReportDocumentDto? Document { get; set; }

        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }

    // Key: (PathologistId, ParcelCode)
    private readonly Dictionary<(int pathologistId, string code), ParcelRow> _parcelsByKey = new();
    private readonly Dictionary<int, ParcelRow> _parcelsById = new();

    private readonly Dictionary<int, ReportRow> _reports = new();

    internal object Sync => _lock;

    internal byte[] NextRv()
    {
        var v = Interlocked.Increment(ref _rv);
        return BitConverter.GetBytes(v);
    }

    internal ReportRow[] SnapshotReports()
    {
        lock (_lock) return _reports.Values.ToArray();
    }

    internal ParcelRow[] SnapshotParcels()
    {
        lock (_lock) return _parcelsById.Values.ToArray();
    }

    internal bool TryGetReport(PathologyReportId id, out ReportRow row)
    {
        lock (_lock) return _reports.TryGetValue(id.Value, out row!);
    }

    internal bool TryGetParcelByKey(PathologistId pid, string code, out ParcelRow row)
    {
        lock (_lock) return _parcelsByKey.TryGetValue((pid.Value, code), out row!);
    }

    internal bool TryGetParcelById(PathologyParcelId id, out ParcelRow row)
    {
        lock (_lock) return _parcelsById.TryGetValue(id.Value, out row!);
    }

    internal PathologyParcelId CreateParcel(PathologyParcelCreateRequestDto req, out ParcelRow row)
    {
        lock (_lock)
        {
            // Generate sortable, per-pathologist unique code
            var baseCode = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            var code = baseCode;
            var n = 1;
            while (_parcelsByKey.ContainsKey((req.PathologistId.Value, code)))
            {
                code = $"{baseCode}-{n:00}";
                n++;
            }

            var id = new PathologyParcelId(_nextParcelId++);
            row = new ParcelRow
            {
                Id = id,
                PathologistId = req.PathologistId,
                ParcelCode = code,
                DispatchedAtUtc = req.DispatchedAtUtc,
                CourierName = req.CourierName,
                TrackingNumber = req.TrackingNumber,
                Notes = req.Notes,
                CreatedAtUtc = DateTime.UtcNow,
                RowVersion = NextRv()
            };

            _parcelsById[id.Value] = row;
            _parcelsByKey[(req.PathologistId.Value, code)] = row;

            return id;
        }
    }

    internal PathologyReportId CreateReport(PathologyReportUpsertRequestDto req, out ReportRow row)
    {
        lock (_lock)
        {
            // (Optional) enforce 1 report per (PathologistId, EndoscopyId)
            var exists = _reports.Values.Any(r => r.PathologistId == req.PathologistId && r.EndoscopyId == req.EndoscopyId);
            if (exists)
                throw new InvalidOperationException("A pathology report already exists for this pathologist + endoscopy.");

            var id = new PathologyReportId(_nextReportId++);
            row = new ReportRow
            {
                Id = id,
                PatientId = req.PatientId,
                EndoscopyId = req.EndoscopyId,
                PathologistId = req.PathologistId,

                SentAtUtc = req.SentAtUtc,
                ReceivedAtUtc = req.ReceivedAtUtc,

                Notes = req.Notes,
                ClinicalInfo = req.ClinicalInfo,
                MacroscopyText = req.MacroscopyText,
                DiagnosisText = req.DiagnosisText,

                Status = req.Status,
                IsUrgent = req.IsUrgent,

                DocumentFileId = req.DocumentFileId,
                DocumentKind = req.DocumentKind,
                DocumentFileName = req.DocumentFileName,
                DocumentContentType = req.DocumentContentType,
                Document = req.Document,

                RowVersion = NextRv()
            };

            _reports[id.Value] = row;
            return id;
        }
    }

    internal void UpdateReport(PathologyReportUpsertRequestDto req, ReportRow row)
    {
        // caller holds lock
        row.PatientId = req.PatientId;
        row.EndoscopyId = req.EndoscopyId;
        row.PathologistId = req.PathologistId;

        row.SentAtUtc = req.SentAtUtc;
        row.ReceivedAtUtc = req.ReceivedAtUtc;

        row.Notes = req.Notes;
        row.ClinicalInfo = req.ClinicalInfo;
        row.MacroscopyText = req.MacroscopyText;
        row.DiagnosisText = req.DiagnosisText;

        row.Status = req.Status;
        row.IsUrgent = req.IsUrgent;

        row.DocumentFileId = req.DocumentFileId;
        row.DocumentKind = req.DocumentKind;
        row.DocumentFileName = req.DocumentFileName;
        row.DocumentContentType = req.DocumentContentType;
        row.Document = req.Document;

        row.RowVersion = NextRv();
    }

    internal bool DeleteReport(PathologyReportId id)
    {
        lock (_lock)
        {
            if (!_reports.TryGetValue(id.Value, out var r))
                return false;

            // Remove from any parcel membership
            if (!string.IsNullOrWhiteSpace(r.DispatchParcelCode))
            {
                if (_parcelsByKey.TryGetValue((r.PathologistId.Value, r.DispatchParcelCode!), out var parcel))
                    parcel.ReportIds.Remove(id.Value);
            }

            return _reports.Remove(id.Value);
        }
    }

    internal void AssignReportsToParcel(ParcelRow parcel, IReadOnlyCollection<PathologyReportId> reportIds)
    {
        // caller holds lock
        foreach (var rid in reportIds)
        {
            if (!_reports.TryGetValue(rid.Value, out var r))
                throw new KeyNotFoundException($"Report {rid.Value} not found.");

            if (r.PathologistId != parcel.PathologistId)
                throw new InvalidOperationException("Cannot assign report to a parcel of a different pathologist.");

            // If it was assigned to another parcel, detach from that
            if (!string.IsNullOrWhiteSpace(r.DispatchParcelCode) &&
                _parcelsByKey.TryGetValue((r.PathologistId.Value, r.DispatchParcelCode!), out var oldParcel))
            {
                oldParcel.ReportIds.Remove(rid.Value);
            }

            r.DispatchParcelCode = parcel.ParcelCode;
            r.RowVersion = NextRv();

            parcel.ReportIds.Add(rid.Value);
        }

        parcel.RowVersion = NextRv();
    }

    internal void UnassignReportsFromParcel(ParcelRow parcel, IReadOnlyCollection<PathologyReportId> reportIds)
    {
        // caller holds lock
        foreach (var rid in reportIds)
        {
            if (!_reports.TryGetValue(rid.Value, out var r))
                continue;

            if (r.PathologistId != parcel.PathologistId)
                continue;

            if (r.DispatchParcelCode == parcel.ParcelCode)
            {
                r.DispatchParcelCode = null;
                r.RowVersion = NextRv();
            }

            parcel.ReportIds.Remove(rid.Value);
        }

        parcel.RowVersion = NextRv();
    }
}
