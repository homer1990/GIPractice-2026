namespace GIPractice.Api.Models.Lookups;

public class OrganLookupDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DefaultName { get; set; } = string.Empty; // will be overridden by localization if available
}

public class OrganAreaLookupDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DefaultName { get; set; } = string.Empty;
    public List<string> OrganCodes { get; set; } = [];
}

public class FindingLookupDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Standard { get; set; } = string.Empty;
}

public class PreparationProtocolLookupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int? EndoscopyType { get; set; }        // <-- nullable
    public string? EndoscopyTypeName { get; set; } // <-- nullable too

    public bool IncludesBowelCleaning { get; set; }
    public int? MinimumFastingHours { get; set; }
    public string? Instructions { get; set; }
    public bool IsActive { get; set; }
}


/// <summary>
/// For enums like Organs, EndoscopyType, etc.
/// </summary>
public class EnumLookupDto
{
    public int Value { get; set; }
    public string Name { get; set; } = string.Empty; //Localized label
}
