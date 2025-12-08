namespace GIPractice.Core.Entities;

public class Localization : BaseEntity
{
    public int FieldNameId { get; set; }
    public string CultureName { get; set; } = "en";
    public string Value { get; set; } = string.Empty;

    public FieldName? FieldName { get; set; }
}
