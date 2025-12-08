using System;
using System.Linq;
using System.Windows;
using GIPractice.Client;

namespace GIPractice.Wpf;

public static class ThemeManager
{
    private static readonly Uri LightUri = new("Themes/Colors.Light.xaml", UriKind.Relative);
    private static readonly Uri DarkUri = new("Themes/Colors.Dark.xaml", UriKind.Relative);

    public static void ApplyTheme(string? themeName)
    {
        var app = Application.Current;
        if (app == null) return;

        var dictionaries = app.Resources.MergedDictionaries;

        // Remove any existing color dictionaries (Colors.*)
        var toRemove = dictionaries
            .Where(d => d.Source != null &&
                        d.Source.OriginalString.Contains("Themes/Colors.", StringComparison.OrdinalIgnoreCase))
            .ToList();

        foreach (var d in toRemove)
            dictionaries.Remove(d);

        // Decide which theme to load
        var name = string.IsNullOrWhiteSpace(themeName) ? "Light" : themeName;
        Uri uri = name.Equals("Dark", StringComparison.OrdinalIgnoreCase) ? DarkUri : LightUri;

        // Insert color dictionary at the beginning so Brushes override generic ones if needed
        dictionaries.Insert(0, new ResourceDictionary { Source = uri });
    }
}
