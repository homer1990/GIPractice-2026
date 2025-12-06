using GIPractice.Core.Abstractions;
using GIPractice.Core.Enums;

namespace GIPractice.Core.Entities;

public class PreparationProtocol : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional EndoscopyType this protocol is intended for
    /// (e.g. Colonoscopy for bowel prep).
    /// </summary>
    public EndoscopyType? EndoscopyType { get; set; }

    /// <summary>
    /// Full instructions (plain text / markdown).
    /// </summary>
    public string Instructions { get; set; } = string.Empty;

    /// <summary>
    /// True if this protocol includes bowel cleansing regimen.
    /// </summary>
    public bool IncludesBowelCleaning { get; set; }

    /// <summary>
    /// Minimum fasting window before procedure, in hours.
    /// </summary>
    public int? MinimumFastingHours { get; set; }

    public bool IsActive { get; set; } = true;
}
