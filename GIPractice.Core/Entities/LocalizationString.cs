using GIPractice.Core.Abstractions;

namespace GIPractice.Core.Entities;

public class LocalizationString : BaseEntity
{
    public string Domain { get; set; } = string.Empty;   // "Organ", "OrganArea", "EndoscopyType", "TestType", etc.
    public string Code { get; set; } = string.Empty;     // e.g. "GEJ", "STOMACH", "COLON_HEPATIC_FLEX"
    public string Language { get; set; } = "en";         // "en", "el", ...
    public string Text { get; set; } = string.Empty;     // actual label
}
