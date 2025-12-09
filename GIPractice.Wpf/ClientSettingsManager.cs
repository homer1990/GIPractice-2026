using System;
using GIPractice.Client;

namespace GIPractice.Wpf;

public class ClientSettingsManager
{
    private readonly IClientSettingsStore _store;

    public ClientSettingsManager(IClientSettingsStore store, ClientSettings defaults)
    {
        _store = store;
        Settings = _store.Load(defaults);
    }

    public ClientSettings Settings { get; private set; }

    public DatabaseOptions CreateDatabaseOptions()
    {
        return new DatabaseOptions
        {
            BaseAddress = new Uri(Settings.ApiBaseAddress, UriKind.Absolute),
            HealthEndpoint = Settings.HealthEndpoint,
            ConnectivityCheckInterval = TimeSpan.FromSeconds(Settings.ConnectivityIntervalSeconds),
            InactivityTimeout = TimeSpan.FromMinutes(Settings.InactivityMinutes)
        };
    }

    public void UpdateServerAddress(string serverUrl)
    {
        var normalized = NormalizeServerUrl(serverUrl);
        Settings.ApiBaseAddress = normalized.AbsoluteUri;
        _store.Save(Settings);
    }

    private static Uri NormalizeServerUrl(string serverUrl)
    {
        if (!Uri.TryCreate(serverUrl, UriKind.Absolute, out var uri))
        {
            throw new ArgumentException("Server URL must be an absolute URI.", nameof(serverUrl));
        }

        if (!uri.AbsoluteUri.EndsWith("/", StringComparison.Ordinal))
        {
            uri = new Uri(uri.AbsoluteUri + "/");
        }

        return uri;
    }
}
