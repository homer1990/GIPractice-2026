using GIPractice.Core.Abstractions;
using GIPractice.Core.Enums;

namespace GIPractice.Core.Entities;

public class Medicine : BaseEntity
{
    public string BrandName { get; set; } = string.Empty;

    public DrugType Type { get; set; } = DrugType.None;

    public List<ActiveSubstance> ActiveSubstances { get; set; } = [];

    public List<Treatment> Treatments { get; set; } = [];
}
