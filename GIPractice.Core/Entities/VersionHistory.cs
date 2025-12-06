namespace GIPractice.Core.Entities;

public enum VersionChangeType
{
    Created = 0,
    Updated = 1,
    Deleted = 2,
    Restored = 3
}

public class VersionHistory
{
    public long Id { get; set; }

    public string EntityName { get; set; } = string.Empty;
    public int EntityId { get; set; }

    public int Version { get; set; }
    public VersionChangeType ChangeType { get; set; }

    public string SnapshotJson { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
}
