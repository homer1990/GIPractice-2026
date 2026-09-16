namespace GIPractice.Server.Clinical;

public enum AnatomicalSiteKind
{
    Organ = 1,
    Region = 2,
    Landmark = 3,
    Other = 99
}

// Small GI-focused controlled vocabulary. Code is a stable application code such as
// STOMACH_ANTRUM or GEJ. ParentId provides hierarchy so a query for STOMACH can include
// its child regions without text matching.
public sealed record AnatomicalSite(
    Guid Id,
    string Code,
    string DisplayName,
    AnatomicalSiteKind Kind,
    Guid? ParentId = null,
    int SortOrder = 0);

// Human input is matched against aliases; stored clinical relations point to the
// canonical AnatomicalSite instead of preserving spelling variants as semantics.
public sealed record AnatomicalSiteAlias(
    Guid Id,
    Guid AnatomicalSiteId,
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
