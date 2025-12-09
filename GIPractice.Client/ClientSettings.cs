using System;

namespace GIPractice.Client;

public class ClientSettings
{
    public string ApiBaseAddress { get; set; } = "https://localhost:7028/";
    public string HealthEndpoint { get; set; } = "health";
    public int InactivityMinutes { get; set; } = 15;
    public int ConnectivityIntervalSeconds { get; set; } = 10;
    public string Language { get; set; } = "en";
}

public interface IClientSettingsStore
{
    ClientSettings Load(ClientSettings defaults);
    void Save(ClientSettings settings);
}
