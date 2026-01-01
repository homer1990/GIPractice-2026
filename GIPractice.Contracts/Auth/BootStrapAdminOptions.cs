namespace GIPractice.Api.Auth;

public sealed class BootstrapAdminOptions
{
    public bool Enabled { get; set; } = false;
    public string UserName { get; set; } = "admin";
    public string Password { get; set; } = "";
    public string? DisplayName { get; set; } = "Administrator";
}
