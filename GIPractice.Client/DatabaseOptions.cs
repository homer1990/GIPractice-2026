using System;

namespace GIPractice.Client;

public class DatabaseOptions
{
    public Uri BaseAddress { get; set; } = new("https://localhost:5001/");
    public string HealthEndpoint { get; set; } = "health";
    public TimeSpan ConnectivityCheckInterval { get; set; } = TimeSpan.FromSeconds(10);
    public TimeSpan InactivityTimeout { get; set; } = TimeSpan.FromMinutes(15);
}
