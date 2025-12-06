using GIPractice.Core.Abstractions;

namespace GIPractice.Core.Entities;

public class MediaFile : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string PseudoLink { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;

    public DateTime? ReceivedAtUtc { get; set; } = DateTime.UtcNow;
}
