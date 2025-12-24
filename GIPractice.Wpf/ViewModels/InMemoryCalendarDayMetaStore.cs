using System;
using System.Collections.Generic;

namespace GIPractice.Wpf.ViewModels;

public interface ICalendarDayMetaStore
{
    DayMetaVm GetOrCreate(DateTime date);
}

public sealed class InMemoryCalendarDayMetaStore : ICalendarDayMetaStore
{
    private readonly Dictionary<DateTime, DayMetaVm> _map = new();

    public DayMetaVm GetOrCreate(DateTime date)
    {
        var d = date.Date;
        if (_map.TryGetValue(d, out var meta))
            return meta;

        meta = new DayMetaVm(d);
        _map[d] = meta;
        return meta;
    }
}
