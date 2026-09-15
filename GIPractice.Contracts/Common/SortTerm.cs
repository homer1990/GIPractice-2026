namespace GIPractice.Contracts.Common;

/// <summary>
/// A stable API field name plus direction. The API resolves Field through its
/// server-side field catalog; clients never send entity property paths.
/// </summary>
public sealed record SortTerm(string Field, SortDirection Direction = SortDirection.Ascending);
