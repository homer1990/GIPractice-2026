using System.ComponentModel;
using System.Globalization;

namespace GIPractice.Client.Core;

public interface IStringLocalizer : INotifyPropertyChanged
{
    string this[string key] { get; }
    CultureInfo CurrentCulture { get; set; }
}
