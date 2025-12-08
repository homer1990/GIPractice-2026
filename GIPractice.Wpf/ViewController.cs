using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace GIPractice.Wpf;

public class ViewController
{
    private readonly ResourceDictionary _viewTemplates = new() { Source = new Uri("Views/ViewTemplates.xaml", UriKind.Relative) };
    private readonly ResourceDictionary _commonTemplates = new() { Source = new Uri("Views/CommonTemplates.xaml", UriKind.Relative) };

    public void Load(Application app)
    {
        var dictionaries = app.Resources.MergedDictionaries;
        AddIfMissing(dictionaries, _viewTemplates);
        AddIfMissing(dictionaries, _commonTemplates);
    }

    private static void AddIfMissing(Collection<ResourceDictionary> dictionaries, ResourceDictionary dictionary)
    {
        if (dictionaries.Any(d => d.Source == dictionary.Source))
            return;

        dictionaries.Add(dictionary);
    }
}
