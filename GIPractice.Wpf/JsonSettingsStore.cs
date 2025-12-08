using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using GIPractice.Client;

namespace GIPractice.Wpf;

public class JsonSettingsStore : ISettingsStore
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    public JsonSettingsStore()
    {
        var dir = GetBaseDirectory();
        Directory.CreateDirectory(dir);
        _filePath = Path.Combine(dir, "settings.json");
    }

    private static string GetBaseDirectory()
    {
        // WPF = Windows today, but this makes it reusable
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            // %APPDATA%\GIPracticeClient
            return Path.Combine(appData, "GIPracticeClient");
        }
        else
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            // ~/.GIPracticeClient
            return Path.Combine(home, ".GIPracticeClient");
        }
    }

    public async Task<ClientSettings> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
            return new ClientSettings();

        await using var stream = File.OpenRead(_filePath);
        var settings = JsonSerializer.Deserialize<ClientSettings>(stream, _options);
        return settings ?? new ClientSettings();
    }

    public async Task SaveAsync(ClientSettings settings, CancellationToken cancellationToken = default)
    {
        await using var stream = File.Create(_filePath);
        JsonSerializer.Serialize(stream, settings, _options);
    }
}
