using Microsoft.AspNetCore.Identity;

namespace GIPractice.Core.Entities.Identity;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }

    /// <summary>
    /// Soft enable/disable flag for application access.
    /// </summary>
    public bool IsActive { get; set; } = true;

    public DateTimeOffset? LastLoginAtUtc { get; set; }
}
