namespace GIPractice.Contracts.Users;

public sealed record UserDto(
    string Id,
    string UserName,
    string? DisplayName,
    string? Email,
    string? PhoneNumber,
    bool IsActive,
    DateTimeOffset? LastLoginAtUtc,
    IReadOnlyList<string> Roles,
    string? ConcurrencyStamp);
