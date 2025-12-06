using GIPractice.Core.Abstractions;

namespace GIPractice.Core.Entities;

public class OrganArea : BaseEntity
{
    /// <summary>Stable code (e.g. "GEJ", "DUO_BULB", "COLON_HEPATIC_FLEX").</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Default (fallback) name, e.g. English.</summary>
    public string DefaultName { get; set; } = string.Empty;

    // many-to-many Organ <-> OrganArea
    public List<OrganAreaOrgan> OrganAreaOrgans { get; set; } = new();

    // existing relationships
    public List<Observation> Observations { get; set; } = new();
    public List<BiopsyBottle> BiopsyBottles { get; set; } = new();
}