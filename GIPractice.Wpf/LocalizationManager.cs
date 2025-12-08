using GIPractice.Client.Core;

namespace GIPractice.Wpf;

public class LocalizationManager
{
    private readonly ILocalizationCatalog _catalog;
    private readonly string _language;

    public LocalizationManager(ILocalizationCatalog catalog)
    {
        _catalog = catalog;
        _language = "en";
    }

    public LocalizationManager(ILocalizationCatalog catalog, string language)
    {
        _catalog = catalog;
        _language = string.IsNullOrWhiteSpace(language) ? "en" : language;
    }

    public string this[string key] => _catalog.GetString(key, _language) ?? key;

    public string Field(string table, string field) => _catalog.GetFieldLabel(table, field, _language) ?? field;
}
