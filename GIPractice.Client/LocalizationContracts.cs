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
