using System;
using System.Linq;
using System.Windows;

namespace GIPractice.Wpf;

public static class LanguageManager
{
    private static readonly Uri EnUri = new("Localization/Strings.en.xaml", UriKind.Relative);
    private static readonly Uri ElUri = new("Localization/Strings.el.xaml", UriKind.Relative);

    public static void ApplyLanguage(string? cultureName)
    {
        var app = Application.Current;
        if (app is null) return;

        var dictionaries = app.Resources.MergedDictionaries;

        // Remove existing string dictionaries
        var toRemove = dictionaries
            .Where(d => d.Source != null &&
                        d.Source.OriginalString.Contains("Localization/Strings.", StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var d in toRemove)
            dictionaries.Remove(d);

        // Decide language (default = en)
        var lang = (cultureName ?? "en")[..Math.Min(2, cultureName?.Length ?? 2)].ToLowerInvariant();

        Uri uri = lang == "el" ? ElUri : EnUri;

        dictionaries.Add(new ResourceDictionary { Source = uri });
    }
}
