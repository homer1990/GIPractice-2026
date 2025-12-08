using GIPractice.Core.Abstractions;
namespace GIPractice.Core.Entities;

public class FieldName : BaseEntity
{
    public string TableName { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string DefaultText { get; set; } = string.Empty;

    public ICollection<Localization> Localizations { get; set; } = new List<Localization>();
}
