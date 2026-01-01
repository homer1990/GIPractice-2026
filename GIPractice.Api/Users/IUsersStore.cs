using GIPractice.Contracts.Common;
using GIPractice.Contracts.Users;

namespace GIPractice.Api.Users;

public interface IUsersStore
{
    Task<ResultDto<PagedResultDto<UserListItemDto>>> SearchAsync(
        UserSearchRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<UserDto>> GetAsync(
        string id,
        CancellationToken cancellationToken = default);

    Task<ResultDto<string>> CreateAsync(
        UserCreateRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> UpdateAsync(
        string id,
        UserUpdateRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> SetPasswordAsync(
        string id,
        UserSetPasswordRequestDto request,
        CancellationToken cancellationToken = default);

    Task<ResultDto<bool>> DeleteAsync(
        string id,
        CancellationToken cancellationToken = default);
}
