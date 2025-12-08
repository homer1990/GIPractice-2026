using System.Collections.Generic;

namespace GIPractice.Client;

public record FieldNameDescriptor(string Table, string Field, string Key);

public record FieldLocalization(string Language, string Table, string Field, string Value);

public interface ILocalizationCatalog
{
    string? GetFieldLabel(string table, string field, string language);
    string? GetString(string key, string language);
    IReadOnlyDictionary<string, string> GetFieldsForTable(string table, string language);
}

/// <summary>
/// Simple localizer interface used by the client for binding UI strings.
/// </summary>
public interface IStringLocalizer
{
    /// <summary>Returns a localized string for the given key.</summary>
    string this[string key] { get; }

    /// <summary>Gets a localized string for the given key.</summary>
    string GetString(string key);

    /// <summary>Gets a localized field label for the specified table/field pair.</summary>
    string GetFieldLabel(string table, string field);

    /// <summary>Gets all field labels for a table.</summary>
    IReadOnlyDictionary<string, string> GetFieldsForTable(string table);
}
