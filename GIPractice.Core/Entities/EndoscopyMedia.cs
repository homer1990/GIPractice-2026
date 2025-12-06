using GIPractice.Core.Abstractions;

namespace GIPractice.Core.Entities;

public class EndoscopyMedia : MediaFile
{
    public int EndoscopyId { get; set; }
    public Endoscopy Endoscopy { get; set; } = null!;
}
