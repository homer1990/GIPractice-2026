using System;
using System.IO;
using System.Text.Json;
using GIPractice.Client;
using Microsoft.Win32;

namespace GIPractice.Wpf;

public class RegistrySettingsStore : IClientSettingsStore
{
    private const string RegistryPath = "Software\\GIPractice";
    private readonly string _settingsFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "GIPractice",
        "settings.json");

    public ClientSettings Load(ClientSettings defaults)
    {
        var fromRegistry = LoadFromRegistry(defaults);
        if (fromRegistry is not null)
        {
            return fromRegistry;
        }

        var fromFile = LoadFromFile(defaults);
        if (fromFile is not null)
        {
            SaveToRegistry(fromFile);
            return fromFile;
        }

        return defaults;
    }

    public void Save(ClientSettings settings)
    {
        SaveToRegistry(settings);
        PersistToFile(settings);
    }

    private static void SaveToRegistry(ClientSettings settings)
    {
        using var key = Registry.CurrentUser.CreateSubKey(RegistryPath);
        key?.SetValue(nameof(ClientSettings.ApiBaseAddress), settings.ApiBaseAddress);
        key?.SetValue(nameof(ClientSettings.HealthEndpoint), settings.HealthEndpoint);
        key?.SetValue(nameof(ClientSettings.InactivityMinutes), settings.InactivityMinutes, RegistryValueKind.DWord);
        key?.SetValue(nameof(ClientSettings.ConnectivityIntervalSeconds), settings.ConnectivityIntervalSeconds, RegistryValueKind.DWord);
        key?.SetValue(nameof(ClientSettings.Language), settings.Language);
    }

    private ClientSettings? LoadFromRegistry(ClientSettings defaults)
    {
        using var key = Registry.CurrentUser.OpenSubKey(RegistryPath);
        if (key is null)
        {
            return null;
        }

        var apiBaseAddress = key.GetValue(nameof(ClientSettings.ApiBaseAddress)) as string;
        var healthEndpoint = key.GetValue(nameof(ClientSettings.HealthEndpoint)) as string;
        var inactivityMinutes = key.GetValue(nameof(ClientSettings.InactivityMinutes)) as int?;
        var connectivitySeconds = key.GetValue(nameof(ClientSettings.ConnectivityIntervalSeconds)) as int?;
        var language = key.GetValue(nameof(ClientSettings.Language)) as string;

        if (apiBaseAddress is null && healthEndpoint is null && inactivityMinutes is null && connectivitySeconds is null && language is null)
        {
            return null;
        }

        return new ClientSettings
        {
            ApiBaseAddress = apiBaseAddress ?? defaults.ApiBaseAddress,
            HealthEndpoint = healthEndpoint ?? defaults.HealthEndpoint,
            InactivityMinutes = inactivityMinutes ?? defaults.InactivityMinutes,
            ConnectivityIntervalSeconds = connectivitySeconds ?? defaults.ConnectivityIntervalSeconds,
            Language = language ?? defaults.Language
        };
    }

    private ClientSettings? LoadFromFile(ClientSettings defaults)
    {
        if (!File.Exists(_settingsFilePath))
        {
            return null;
        }

        try
        {
            var json = File.ReadAllText(_settingsFilePath);
            var fromFile = JsonSerializer.Deserialize<ClientSettings>(json);
            if (fromFile is null)
            {
                return null;
            }

            return Merge(defaults, fromFile);
        }
        catch (IOException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private void PersistToFile(ClientSettings settings)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_settingsFilePath)!);
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_settingsFilePath, json);
        }
        catch (IOException)
        {
            // Best effort persistence for local file.
        }
    }

    private static ClientSettings Merge(ClientSettings defaults, ClientSettings overrides)
    {
        return new ClientSettings
        {
            ApiBaseAddress = string.IsNullOrWhiteSpace(overrides.ApiBaseAddress) ? defaults.ApiBaseAddress : overrides.ApiBaseAddress,
            HealthEndpoint = string.IsNullOrWhiteSpace(overrides.HealthEndpoint) ? defaults.HealthEndpoint : overrides.HealthEndpoint,
            InactivityMinutes = overrides.InactivityMinutes == 0 ? defaults.InactivityMinutes : overrides.InactivityMinutes,
            ConnectivityIntervalSeconds = overrides.ConnectivityIntervalSeconds == 0 ? defaults.ConnectivityIntervalSeconds : overrides.ConnectivityIntervalSeconds,
            Language = string.IsNullOrWhiteSpace(overrides.Language) ? defaults.Language : overrides.Language
        };
    }
}
