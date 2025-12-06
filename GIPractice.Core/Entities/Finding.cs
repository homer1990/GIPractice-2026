using GIPractice.Core.Abstractions;
using static System.Net.Mime.MediaTypeNames;

namespace GIPractice.Core.Entities;

public class Finding : BaseEntity
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    /// <summary>Standard coding system, e.g. "WHO", "SNOMED".</summary>
    public string Standard { get; set; } = "WHO";

    public List<Endoscopy> Endoscopies { get; set; } = [];
    public List<Test> Tests { get; set; } = [];
    public List<InfaiTest> InfaiTests { get; set; } = [];

    public ICollection<Observation> Observations { get; set; } = [];
}
