using GIPractice.Core.Enums;

namespace GIPractice.Api.Models;

public class MedicineDto
{
    public int Id { get; set; }

    public string BrandName { get; set; } = string.Empty;

    public DrugType Type { get; set; } = DrugType.None;

    public List<int> ActiveSubstanceIds { get; set; } = [];
    public List<string> ActiveSubstanceNames { get; set; } = [];
}

public class MedicineCreateUpdateDto
{
    public string BrandName { get; set; } = string.Empty;

    public DrugType Type { get; set; } = DrugType.None;

    public List<int> ActiveSubstanceIds { get; set; } = [];
}