namespace GIPractice.Client;

public class ClientSettings
{
    /// <summary>
    /// Base URL of the API, e.g. "https://localhost:7028/".
    /// </summary>
    public string? ApiBaseUrl { get; set; }

    /// <summary>
    /// Last successfully used username.
    /// </summary>
    public string? LastUserName { get; set; }

    /// <summary>
    /// UI culture name, e.g. "en", "el".
    /// </summary>
    public string UICulture { get; set; } = "en";

    /// <summary>
    /// Theme name, e.g. "Light", "Dark".
    /// </summary>
    public string Theme { get; set; } = "Light";
}
