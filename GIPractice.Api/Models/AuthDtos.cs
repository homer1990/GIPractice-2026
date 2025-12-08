namespace GIPractice.Api.Models;

public class LoginRequestDto
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class CurrentUserDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string Email { get; set; } = string.Empty;
    public IReadOnlyList<string> Roles { get; set; } = [];
}

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }

    public CurrentUserDto User { get; set; } = null!;
}
