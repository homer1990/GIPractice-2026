using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace GIPractice.Client;

public class SettingsViewModel(ISettingsStore store) : INotifyPropertyChanged
{
    private readonly ISettingsStore _store = store;
    private ClientSettings _settings = new();

    private string _apiBaseUrl = string.Empty;
    private string _uiCulture = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string ApiBaseUrl
    {
        get => _apiBaseUrl;
        set
        {
            if (_apiBaseUrl == value) return;
            _apiBaseUrl = value;
            OnPropertyChanged();
        }
    }

    public string UICulture
    {
        get => _uiCulture;
        set
        {
            if (_uiCulture == value) return;
            _uiCulture = value;
            OnPropertyChanged();
        }
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _settings = await _store.LoadAsync(cancellationToken) ?? new ClientSettings();

        ApiBaseUrl = _settings.ApiBaseUrl ?? string.Empty;
        UICulture = _settings.UICulture ?? string.Empty;
    }

    public async Task SaveAsync(CancellationToken cancellationToken = default)
    {
        _settings.ApiBaseUrl = ApiBaseUrl;
        _settings.UICulture = UICulture;

        await _store.SaveAsync(_settings, cancellationToken);
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
