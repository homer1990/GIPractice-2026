using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;

namespace GIPractice.Client.Localization;

public class LocalizationBindingService : IStringLocalizer
{
    private readonly GiPracticeApiClient _client;
    private readonly ConcurrentDictionary<string, string> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly SemaphoreSlim _lock = new(1, 1);

    public LocalizationBindingService(GiPracticeApiClient client)
    {
        _client = client;
        _currentCulture = CultureInfo.CurrentUICulture;
    }

    private CultureInfo _currentCulture;
    public CultureInfo CurrentCulture
    {
        get => _currentCulture;
        set
        {
            if (_currentCulture.Equals(value)) return;
            _currentCulture = value;
            _cache.Clear();
            OnPropertyChanged(null);
        }
    }

    public string this[string key]
    {
        get
        {
            if (_cache.TryGetValue(key, out var value))
                return value;

            _ = LoadAsync(key);
            return key;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private async Task LoadAsync(string key)
    {
        var parts = key.Split('.', 2, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
            return;

        var table = parts[0];
        var field = parts[1];

        await _lock.WaitAsync();
        try
        {
            if (_cache.ContainsKey(key))
                return;

            var response = await _client.GetLocalizationAsync(table, field, _currentCulture.Name);
            var value = response?.Value ?? field;
            _cache[key] = value;
        }
        finally
        {
            _lock.Release();
        }

        OnPropertyChanged(key);
    }

    private void OnPropertyChanged(string? key)
    {
        if (PropertyChanged == null)
            return;

        // WPF bindings to indexers listen for "Item[]"; raising a specific key name
        // would be ignored by the binding engine.
        PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Item[]"));
    }
}
