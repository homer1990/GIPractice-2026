using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using GIPractice.Client.Core;

namespace GIPractice.ApiClient;

public class JsonLocalizationCatalog : ILocalizationCatalog
{
    private readonly Dictionary<string, LocalizationDocument> _documents;

    public JsonLocalizationCatalog()
    {
        _documents = LoadDocuments();
    }

    public string? GetFieldLabel(string table, string field, string language)
    {
        if (!_documents.TryGetValue(language.ToLowerInvariant(), out var doc))
        {
            return null;
        }

        if (doc.Fields.TryGetValue(table, out var fields) && fields.TryGetValue(field, out var value))
        {
            return value;
        }

        return null;
    }

    public IReadOnlyDictionary<string, string> GetFieldsForTable(string table, string language)
    {
        if (_documents.TryGetValue(language.ToLowerInvariant(), out var doc) && doc.Fields.TryGetValue(table, out var fields))
        {
            return fields;
        }

        return new Dictionary<string, string>();
    }

    public string? GetString(string key, string language)
    {
        if (!_documents.TryGetValue(language.ToLowerInvariant(), out var doc))
        {
            return null;
        }

        return doc.Ui.TryGetValue(key, out var value) ? value : null;
    }

    private static Dictionary<string, LocalizationDocument> LoadDocuments()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resources = assembly.GetManifestResourceNames()
            .Where(name => name.EndsWith(".json", System.StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var result = new Dictionary<string, LocalizationDocument>();
        foreach (var resource in resources)
        {
            using var stream = assembly.GetManifestResourceStream(resource);
            if (stream is null)
            {
                continue;
            }

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var doc = JsonSerializer.Deserialize<LocalizationDocument>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (doc is null || string.IsNullOrWhiteSpace(doc.Language))
            {
                continue;
            }

            result[doc.Language.ToLowerInvariant()] = doc;
        }

        return result;
    }

    private class LocalizationDocument
    {
        public string Language { get; set; } = string.Empty;
        public Dictionary<string, string> Ui { get; set; } = new();
        public Dictionary<string, Dictionary<string, string>> Fields { get; set; } = new();
    }
}
