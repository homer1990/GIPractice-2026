using GIPractice.Contracts.Common;
using GIPractice.Contracts.Users;

namespace GIPractice.Api.Users;

public sealed class UsersService(IUsersStore store) : IUsersService
{
    private readonly IUsersStore _store = store;

    public Task<ResultDto<PagedResultDto<UserListItemDto>>> SearchAsync(
        UserSearchRequestDto request,
        CancellationToken cancellationToken = default)
        => _store.SearchAsync(request, cancellationToken);

    public Task<ResultDto<UserDto>> GetAsync(
        string id,
        CancellationToken cancellationToken = default)
        => _store.GetAsync(id, cancellationToken);

    public Task<ResultDto<string>> CreateAsync(
        UserCreateRequestDto request,
        CancellationToken cancellationToken = default)
        => _store.CreateAsync(request, cancellationToken);

    public Task<ResultDto<bool>> UpdateAsync(
        string id,
        UserUpdateRequestDto request,
        CancellationToken cancellationToken = default)
        => _store.UpdateAsync(id, request, cancellationToken);

    public Task<ResultDto<bool>> SetPasswordAsync(
        string id,
        UserSetPasswordRequestDto request,
        CancellationToken cancellationToken = default)
        => _store.SetPasswordAsync(id, request, cancellationToken);

    public Task<ResultDto<bool>> DeleteAsync(
        string id,
        CancellationToken cancellationToken = default)
        => _store.DeleteAsync(id, cancellationToken);
}
