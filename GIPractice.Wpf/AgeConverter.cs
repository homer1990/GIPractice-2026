using System.Globalization;
using System.Windows.Data;

namespace GIPractice.Wpf;

public sealed class AgeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime birthDate)
        {
            int age = DateTime.Today.Year - birthDate.Year;
            if (birthDate > DateTime.Today.AddYears(-age)) age--;
            return age;
        }
        return "N/A";
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
