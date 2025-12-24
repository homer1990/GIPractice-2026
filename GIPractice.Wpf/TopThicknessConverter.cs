using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GIPractice.Wpf;

public sealed class TopThicknessConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var top = value switch
        {
            int i => i,
            double d => d,
            _ => 0
        };

        return new Thickness(0, top, 0, 0);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
