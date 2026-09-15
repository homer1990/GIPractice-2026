using Microsoft.AspNetCore.Identity;

namespace GIPractice.Infrastructure.Identity;

/// <summary>Authentication persistence model; intentionally not part of the clinical domain.</summary>
public sealed class ApplicationUser : IdentityUser
{
}
