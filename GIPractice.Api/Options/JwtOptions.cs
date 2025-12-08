namespace GIPractice.Api.Options;

public class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;   // >= 32 chars
    public int AccessTokenLifetimeMinutes { get; set; } = 480;
}
