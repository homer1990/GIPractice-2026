namespace GIPractice.Contracts.Users;

public sealed record UserUpdateRequestDto(
    string? UserName,
    string? DisplayName,
    string? Email,
    string? PhoneNumber,
    bool? IsActive,
    IReadOnlyList<string>? Roles,
    string? ConcurrencyStamp);
