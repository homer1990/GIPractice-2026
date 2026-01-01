using GIPractice.Contracts.Auth;
using GIPractice.Contracts.Common;
using GIPractice.Contracts.Users;
using GIPractice.Core.Entities.Identity;
using GIPractice.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Users;

public sealed class EfUsersStore(
    AppDbContext db,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager) : IUsersStore
{
    private readonly AppDbContext _db = db;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;

    public async Task<ResultDto<PagedResultDto<UserListItemDto>>> SearchAsync(
        UserSearchRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var paging = request.Paging ?? new PagedRequestDto(1, 50);
        if (paging.Page < 1)
            return ResultDto<PagedResultDto<UserListItemDto>>.Fail("validation", "Page must be >= 1.");
        if (paging.PageSize < 1 || paging.PageSize > 500)
            return ResultDto<PagedResultDto<UserListItemDto>>.Fail("validation", "PageSize must be 1..500.");

        IQueryable<ApplicationUser> q = _db.Users.AsNoTracking();

        var userNameFilter = request.UserName?.Trim();
        if (!string.IsNullOrWhiteSpace(userNameFilter))
        {
            q = q.Where(u =>
                (u.UserName != null && u.UserName.Contains(userNameFilter)) ||
                (u.DisplayName != null && u.DisplayName.Contains(userNameFilter)));
        }

        if (request.IsActive.HasValue)
            q = q.Where(u => u.IsActive == request.IsActive.Value);

        var roleName = request.Role?.Trim();
        if (!string.IsNullOrWhiteSpace(roleName))
        {
            if (!AppRoles.All.Contains(roleName))
                return ResultDto<PagedResultDto<UserListItemDto>>.Fail("validation", $"Unknown role '{roleName}'.");

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role is null)
                return ResultDto<PagedResultDto<UserListItemDto>>.Ok(new PagedResultDto<UserListItemDto>(
                    Items: [],
                    TotalCount: 0,
                    Page: paging.Page,
                    PageSize: paging.PageSize));

            q =
                from u in q
                join ur in _db.UserRoles on u.Id equals ur.UserId
                where ur.RoleId == role.Id
                select u;
        }

        var total = await q.CountAsync(cancellationToken);

        var users = await q
            .OrderBy(u => u.UserName)
            .Skip((paging.Page - 1) * paging.PageSize)
            .Take(paging.PageSize)
            .ToListAsync(cancellationToken);

        var items = new List<UserListItemDto>(users.Count);
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            items.Add(new UserListItemDto(
                Id: u.Id,
                UserName: u.UserName ?? "",
                DisplayName: u.DisplayName,
                IsActive: u.IsActive,
                LastLoginAtUtc: u.LastLoginAtUtc,
                Roles: roles.ToArray()));
        }

        return ResultDto<PagedResultDto<UserListItemDto>>.Ok(new PagedResultDto<UserListItemDto>(
            Items: items,
            TotalCount: total,
            Page: paging.Page,
            PageSize: paging.PageSize));
    }

    public async Task<ResultDto<UserDto>> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            return ResultDto<UserDto>.Fail("validation", "Id is required.");

        var user = await _db.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user is null)
            return ResultDto<UserDto>.Fail("not_found", "User not found.");

        var roles = await _userManager.GetRolesAsync(user);

        return ResultDto<UserDto>.Ok(ToDto(user, roles.ToArray()));
    }

    public async Task<ResultDto<string>> CreateAsync(UserCreateRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.UserName))
            return ResultDto<string>.Fail("validation", "UserName is required.");
        if (string.IsNullOrWhiteSpace(request.Password))
            return ResultDto<string>.Fail("validation", "Password is required.");
        if (request.Roles is null)
            return ResultDto<string>.Fail("validation", "Roles is required.");
        if (request.Roles.Count == 0)
            return ResultDto<string>.Fail("validation", "At least one role is required.");
        if (request.Roles.Any(r => !AppRoles.All.Contains(r)))
            return ResultDto<string>.Fail("validation", "Roles contains an unknown value.");

        // Ensure known roles exist.
        foreach (var role in request.Roles.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new ApplicationRole { Name = role });
        }

        var user = new ApplicationUser
        {
            UserName = request.UserName.Trim(),
            DisplayName = request.DisplayName?.Trim(),
            Email = request.Email?.Trim(),
            PhoneNumber = request.PhoneNumber?.Trim(),
            IsActive = request.IsActive,
        };

        var create = await _userManager.CreateAsync(user, request.Password);
        if (!create.Succeeded)
            return ResultDto<string>.Fail("validation", string.Join("; ", create.Errors.Select(e => e.Description)));

        var addRoles = await _userManager.AddToRolesAsync(user, request.Roles);
        if (!addRoles.Succeeded)
            return ResultDto<string>.Fail("validation", string.Join("; ", addRoles.Errors.Select(e => e.Description)));

        return ResultDto<string>.Ok(user.Id);
    }

    public async Task<ResultDto<bool>> UpdateAsync(string id, UserUpdateRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            return ResultDto<bool>.Fail("validation", "Id is required.");

        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return ResultDto<bool>.Fail("not_found", "User not found.");

        if (!string.IsNullOrWhiteSpace(request.ConcurrencyStamp) &&
            !string.Equals(request.ConcurrencyStamp, user.ConcurrencyStamp, StringComparison.Ordinal))
        {
            return ResultDto<bool>.Fail("conflict", "User was updated by someone else.");
        }

        if (request.UserName is not null)
        {
            var name = request.UserName.Trim();
            if (string.IsNullOrWhiteSpace(name))
                return ResultDto<bool>.Fail("validation", "UserName cannot be empty.");

            var setName = await _userManager.SetUserNameAsync(user, name);
            if (!setName.Succeeded)
                return ResultDto<bool>.Fail("validation", string.Join("; ", setName.Errors.Select(e => e.Description)));
        }

        if (request.DisplayName is not null)
            user.DisplayName = string.IsNullOrWhiteSpace(request.DisplayName) ? null : request.DisplayName.Trim();

        if (request.Email is not null)
        {
            var email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
            var setEmail = await _userManager.SetEmailAsync(user, email);
            if (!setEmail.Succeeded)
                return ResultDto<bool>.Fail("validation", string.Join("; ", setEmail.Errors.Select(e => e.Description)));
        }

        if (request.PhoneNumber is not null)
        {
            var phone = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();
            var setPhone = await _userManager.SetPhoneNumberAsync(user, phone);
            if (!setPhone.Succeeded)
                return ResultDto<bool>.Fail("validation", string.Join("; ", setPhone.Errors.Select(e => e.Description)));
        }

        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        if (request.Roles is not null)
        {
            if (request.Roles.Count == 0)
                return ResultDto<bool>.Fail("validation", "At least one role is required.");
            if (request.Roles.Any(r => !AppRoles.All.Contains(r)))
                return ResultDto<bool>.Fail("validation", "Roles contains an unknown value.");

            foreach (var role in request.Roles.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new ApplicationRole { Name = role });
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var desired = request.Roles.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

            var toRemove = currentRoles.Where(r => !desired.Contains(r, StringComparer.OrdinalIgnoreCase)).ToArray();
            var toAdd = desired.Where(r => !currentRoles.Contains(r, StringComparer.OrdinalIgnoreCase)).ToArray();

            if (toRemove.Length > 0)
            {
                var removed = await _userManager.RemoveFromRolesAsync(user, toRemove);
                if (!removed.Succeeded)
                    return ResultDto<bool>.Fail("validation", string.Join("; ", removed.Errors.Select(e => e.Description)));
            }

            if (toAdd.Length > 0)
            {
                var added = await _userManager.AddToRolesAsync(user, toAdd);
                if (!added.Succeeded)
                    return ResultDto<bool>.Fail("validation", string.Join("; ", added.Errors.Select(e => e.Description)));
            }
        }

        var updated = await _userManager.UpdateAsync(user);
        if (!updated.Succeeded)
            return ResultDto<bool>.Fail("validation", string.Join("; ", updated.Errors.Select(e => e.Description)));

        return ResultDto<bool>.Ok(true);
    }

    public async Task<ResultDto<bool>> SetPasswordAsync(string id, UserSetPasswordRequestDto request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            return ResultDto<bool>.Fail("validation", "Id is required.");
        if (string.IsNullOrWhiteSpace(request.NewPassword))
            return ResultDto<bool>.Fail("validation", "NewPassword is required.");

        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return ResultDto<bool>.Fail("not_found", "User not found.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var reset = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
        if (!reset.Succeeded)
            return ResultDto<bool>.Fail("validation", string.Join("; ", reset.Errors.Select(e => e.Description)));

        return ResultDto<bool>.Ok(true);
    }

    public async Task<ResultDto<bool>> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
            return ResultDto<bool>.Fail("validation", "Id is required.");

        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return ResultDto<bool>.Ok(true); // idempotent

        var deleted = await _userManager.DeleteAsync(user);
        if (!deleted.Succeeded)
            return ResultDto<bool>.Fail("validation", string.Join("; ", deleted.Errors.Select(e => e.Description)));

        return ResultDto<bool>.Ok(true);
    }

    private static UserDto ToDto(ApplicationUser user, IReadOnlyList<string> roles)
        => new(
            Id: user.Id,
            UserName: user.UserName ?? "",
            DisplayName: user.DisplayName,
            Email: user.Email,
            PhoneNumber: user.PhoneNumber,
            IsActive: user.IsActive,
            LastLoginAtUtc: user.LastLoginAtUtc,
            Roles: roles,
            ConcurrencyStamp: user.ConcurrencyStamp);
}
