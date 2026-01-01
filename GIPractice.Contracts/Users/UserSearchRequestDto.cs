using GIPractice.Contracts.Common;

namespace GIPractice.Contracts.Users;

public sealed record UserSearchRequestDto(
    string? UserName,
    string? Role,
    bool? IsActive,
    PagedRequestDto? Paging);
