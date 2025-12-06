using GIPractice.Core.Enums;

namespace GIPractice.Api.Models;

public class PreparationProtocolDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public EndoscopyType? EndoscopyType { get; set; }

    public string Instructions { get; set; } = string.Empty;

    public bool IncludesBowelCleaning { get; set; }

    public int? MinimumFastingHours { get; set; }

    public bool IsActive { get; set; }
}

public class PreparationProtocolCreateUpdateDto
{
    public string Name { get; set; } = string.Empty;

    public EndoscopyType? EndoscopyType { get; set; }

    public string Instructions { get; set; } = string.Empty;

    public bool IncludesBowelCleaning { get; set; }

    public int? MinimumFastingHours { get; set; }

    /// <summary>
    /// For new protocols this will default to true if not explicitly changed on the client.
    /// </summary>
    public bool IsActive { get; set; } = true;
}
