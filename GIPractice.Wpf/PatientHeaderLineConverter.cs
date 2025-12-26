using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace GIPractice.Wpf;

public sealed class PatientHeaderLineConverter : IMultiValueConverter
{
    private static string? AgeText(object? birth)
    {
        if (birth is null || birth == DependencyProperty.UnsetValue)
            return null;

        DateTime dt = birth switch
        {
            DateTime d => d,
            DateTimeOffset dto => dto.DateTime,
            DateOnly dd => dd.ToDateTime(TimeOnly.MinValue),
            _ => default
        };

        if (dt == default)
            return null;

        var today = DateTime.Today;
        var age = today.Year - dt.Year;
        if (dt.Date > today.AddYears(-age)) age--;

        return age >= 0 ? age.ToString(CultureInfo.CurrentCulture) : null;
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var last = values.ElementAtOrDefault(0)?.ToString();
        var first = values.ElementAtOrDefault(1)?.ToString();
        var father = values.ElementAtOrDefault(2)?.ToString();
        var age = AgeText(values.ElementAtOrDefault(3));
        var pn = values.ElementAtOrDefault(4)?.ToString();

        var parts = new List<string>();

        var name = string.Join(" ", new[] { last, first, father }.Where(s => !string.IsNullOrWhiteSpace(s)));
        if (!string.IsNullOrWhiteSpace(name)) parts.Add(name);

        if (!string.IsNullOrWhiteSpace(age)) parts.Add($"Ηλικία: {age}");
        if (!string.IsNullOrWhiteSpace(pn)) parts.Add($"ΠΑ: {pn}");

        return string.Join(" • ", parts);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
