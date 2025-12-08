using System.Threading;
using System.Threading.Tasks;

namespace GIPractice.Client;

public class ClientSettingsService(ISettingsStore store)
{
    private readonly ISettingsStore _store = store;

    public ClientSettings Current { get; private set; } = new();

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Current = await _store.LoadAsync(cancellationToken);
        }
        catch
        {
            Current = new ClientSettings();
        }

        // Default API URL if none stored yet – adjust if needed
        if (string.IsNullOrWhiteSpace(Current.ApiBaseUrl))
        {
            Current.ApiBaseUrl = "https://localhost:7028/";
        }
    }

    public Task SaveAsync(CancellationToken cancellationToken = default) =>
        _store.SaveAsync(Current, cancellationToken);
}
