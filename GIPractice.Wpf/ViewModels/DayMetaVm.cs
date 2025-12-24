using System;

namespace GIPractice.Wpf.ViewModels;

public sealed class DayMetaVm : ViewModelBase
{
    public DateTime Date { get; }

    private bool _isDayOff;
    public bool IsDayOff
    {
        get => _isDayOff;
        set { if (SetProperty(ref _isDayOff, value)) OnPropertyChanged(nameof(IsClosed)); }
    }

    private bool _isHoliday;
    public bool IsHoliday
    {
        get => _isHoliday;
        set { if (SetProperty(ref _isHoliday, value)) OnPropertyChanged(nameof(IsClosed)); }
    }

    private string? _note;
    public string? Note
    {
        get => _note;
        set => SetProperty(ref _note, value);
    }

    public bool IsClosed => IsHoliday || IsDayOff;

    public DayMetaVm(DateTime date) => Date = date.Date;
}
