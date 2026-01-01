namespace GIPractice.Contracts.Users;

public sealed record UserCreateRequestDto(
    string UserName,
    string? DisplayName,
    string? Email,
    string? PhoneNumber,
    bool IsActive,
    string Password,
    IReadOnlyList<string> Roles);
