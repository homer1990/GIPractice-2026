using GIPractice.Core.Abstractions;

namespace GIPractice.Core.Entities;

public class ActiveSubstance : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public List<Medicine> Medicines { get; set; } = [];
}
