using System;
using System.IO;
using System.Text.Json;

namespace GIPractice.Wpf.Backend;

public interface IClientSettings
{
    string ServerUrl { get; set; }
    void Save();
}

public sealed class ClientSettings : IClientSettings
{
    private const string FileName = "GIPractice.ClientSettings.json";

    public string ServerUrl { get; set; } = "https://localhost:5001";

    public static ClientSettings Load()
    {
        try
        {
            var dir = GetSettingsDirectory();
            var path = Path.Combine(dir, FileName);

            if (!File.Exists(path))
                return new ClientSettings();

            var json = File.ReadAllText(path);
            var settings = JsonSerializer.Deserialize<ClientSettings>(json);
            return settings ?? new ClientSettings();
        }
        catch
        {
            // If anything goes wrong, fall back to defaults.
            return new ClientSettings();
        }
    }

    public void Save()
    {
        try
        {
            var dir = GetSettingsDirectory();
            Directory.CreateDirectory(dir);

            var path = Path.Combine(dir, FileName);
#pragma warning disable CA1869
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
#pragma warning restore CA1869
            {
                WriteIndented = true
            });

            File.WriteAllText(path, json);
        }
        catch
        {
            // Ignore errors; worst case the URL isn't persisted.
        }
    }

    private static string GetSettingsDirectory()
    {
        var root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(root, "GIPractice");
    }
}
