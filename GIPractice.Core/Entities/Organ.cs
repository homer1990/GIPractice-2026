using GIPractice.Core.Abstractions;

namespace GIPractice.Core.Entities;

public class Organ : BaseEntity
{
    /// <summary>Stable code (e.g. "ESOPHAGUS", "STOMACH", "DUODENUM").</summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>Default (fallback) name, e.g. English.</summary>
    public string DefaultName { get; set; } = string.Empty;

    public List<OrganAreaOrgan> OrganAreaOrgans { get; set; } = [];
}
