namespace GIPractice.Contracts.Auth;

public sealed record AuthMeDto(
    string UserId,
    string UserName,
    string? DisplayName,
    IReadOnlyList<string> Roles,
    bool IsActive);
