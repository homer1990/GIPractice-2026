using System.Collections.Generic;

namespace GIPractice.Client;

/// <summary>
/// Provides localized strings by delegating to the configured catalog.
/// </summary>
public sealed class LocalizationBindingService : IStringLocalizer
{
    private readonly ILocalizationCatalog _catalog;
    private readonly string _language;

    public LocalizationBindingService(ILocalizationCatalog catalog, string language = "en")
    {
        _catalog = catalog;
        _language = string.IsNullOrWhiteSpace(language) ? "en" : language;
    }

    public string this[string key] => GetString(key);

    public string GetString(string key) => _catalog.GetString(key, _language) ?? key;

    public string GetFieldLabel(string table, string field) => _catalog.GetFieldLabel(table, field, _language) ?? field;

    public IReadOnlyDictionary<string, string> GetFieldsForTable(string table) => _catalog.GetFieldsForTable(table, _language);
}
