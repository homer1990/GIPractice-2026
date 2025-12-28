namespace GIPractice.Core.Abstractions;

public abstract class BaseEntity : IEntity, IAuditable, ISoftDelete
{
    public int Id { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; } = false;
}
