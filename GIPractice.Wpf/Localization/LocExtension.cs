using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace GIPractice.Wpf.Localization;

[MarkupExtensionReturnType(typeof(string))]
public class LocExtension : MarkupExtension
{
    public string Key { get; set; } = string.Empty;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        // At design-time just show the key
        if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            return Key;

        var app = Application.Current;
        if (app == null)
            return Key;

        var value = app.TryFindResource(Key);
        return value ?? Key;
    }
}
