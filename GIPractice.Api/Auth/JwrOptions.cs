namespace GIPractice.Api.Auth;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "GIPractice";
    public string Audience { get; set; } = "GIPractice";
    public string Key { get; set; } = "";
    public int AccessTokenMinutes { get; set; } = 60;
}
