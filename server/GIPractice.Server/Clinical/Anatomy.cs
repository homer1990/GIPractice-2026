namespace GIPractice.Server.Clinical;

public static class ClinicalLocales
{
    public const string GreekGreece = "el-GR";
    public const string English = "en";
}

public enum AnatomicalSiteKind
{
    Organ = 1,
    Region = 2,
    Landmark = 3,
    Other = 99
}

// Stable, language-neutral clinical concept. Code is the semantic identity used by
// relationships and research queries. Human-readable names live in AnatomicalSiteName.
public sealed record AnatomicalSite(
    Guid Id,
    string Code,
    AnatomicalSiteKind Kind,
    Guid? ParentId = null,
    int SortOrder = 0);

// One localized preferred display name for an anatomical concept. Persistence should
// enforce at most one preferred name per (AnatomicalSiteId, Locale).
public sealed record AnatomicalSiteName(
    Guid Id,
    Guid AnatomicalSiteId,
    string Locale,
    string DisplayName);

// Localized spelling, abbreviation and real-world clinician variants used by
// autocomplete/parser recognition. Aliases are never the semantic identity themselves.
public sealed record AnatomicalSiteAlias(
    Guid Id,
    Guid AnatomicalSiteId,
    string Locale,
    string Alias);

public enum AnatomicalReferencePoint
{
    Incisors = 1,
    AnalVerge = 2,
    Other = 99
}

// Stores what was actually observed. Derived measurements (for example the distance
// between GEJ and diaphragmatic impression) are calculated from observations rather
// than stored as the only source fact.
public sealed record ObservedLandmark(
    Guid Id,
    Guid EndoscopyId,
    Guid AnatomicalSiteId,
    decimal Position,
    string UnitCode,
    AnatomicalReferencePoint ReferencePoint,
    string? Note = null);
