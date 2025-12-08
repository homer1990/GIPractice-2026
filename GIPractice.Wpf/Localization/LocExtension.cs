using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using GIPractice.Client.Core;

namespace GIPractice.Wpf.Localization;

[MarkupExtensionReturnType(typeof(string))]
public class LocExtension : MarkupExtension
{
    public string Table { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string? Key { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var designValue = Key ?? ComposeKey();

        // At design-time just show the key
        if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            return designValue;

        var app = Application.Current;
        if (app == null)
            return designValue;

        var localizer = App.Services.GetService(typeof(IStringLocalizer)) as IStringLocalizer;
        if (localizer == null)
            return designValue;

        var binding = new Binding($"[{ComposeKey()}]")
        {
            Source = localizer,
            Mode = BindingMode.OneWay
        };

        return binding.ProvideValue(serviceProvider);
    }

    private string ComposeKey()
    {
        if (!string.IsNullOrWhiteSpace(Key))
            return Key!;

        return string.IsNullOrWhiteSpace(Table) || string.IsNullOrWhiteSpace(Field)
            ? string.Empty
            : $"{Table}.{Field}";
    }
}
