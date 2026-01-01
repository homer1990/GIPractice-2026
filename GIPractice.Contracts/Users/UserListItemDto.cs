namespace GIPractice.Contracts.Users;

public sealed record UserListItemDto(
    string Id,
    string UserName,
    string? DisplayName,
    bool IsActive,
    DateTimeOffset? LastLoginAtUtc,
    IReadOnlyList<string> Roles);
