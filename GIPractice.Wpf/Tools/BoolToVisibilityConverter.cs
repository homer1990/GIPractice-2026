using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GIPractice.Wpf.Tools
{
    /// <summary>
    /// Converts a bool to Visibility.
    /// true  -> Visible
    /// false -> Collapsed
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// If true, the result is inverted (true -> Collapsed, false -> Visible).
        /// </summary>
        public bool Inverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var flag = false;

            if (value is bool b)
                flag = b;

            if (Inverse)
                flag = !flag;

            return flag ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility v)
            {
                var flag = v == Visibility.Visible;
                return Inverse ? !flag : flag;
            }

            return Binding.DoNothing;
        }
    }
}
