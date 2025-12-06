namespace GIPractice.Api.Models;

public class MediaFileDto
{
    public int Id { get; set; }

    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;

    /// <summary>Relative link (e.g. /media/2025/12/05/guid.docx).</summary>
    public string PseudoLink { get; set; } = string.Empty;

    public DateTime? ReceivedAtUtc { get; set; }
}

public class MediaUploadResultDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string PseudoLink { get; set; } = string.Empty;
}
