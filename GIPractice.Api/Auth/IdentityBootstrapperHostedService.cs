using GIPractice.Contracts.Auth;
using GIPractice.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace GIPractice.Api.Auth;

public sealed class IdentityBootstrapperHostedService(
    IServiceProvider services,
    IWebHostEnvironment env,
    IOptions<BootstrapApiAdminOptions> adminOptions) : IHostedService
{
    private readonly IServiceProvider _services = services;
    private readonly IWebHostEnvironment _env = env;
    private readonly BootstrapApiAdminOptions _adminOpt = adminOptions.Value;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_adminOpt.Enabled)
            return;

        // In production, require a non-trivial bootstrap password.
        if (_env.IsProduction() && string.Equals(_adminOpt.Password, "admin", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Refusing to bootstrap an admin with password 'admin' in Production.");

        if (string.IsNullOrWhiteSpace(_adminOpt.Password))
            throw new InvalidOperationException("BootstrapAdmin:Password is required when BootstrapAdmin:Enabled=true.");

        using var scope = _services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Ensure known roles exist.
        foreach (var role in AppRoles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new ApplicationRole { Name = role });
        }

        // Ensure bootstrap admin exists.
        var admin = await userManager.FindByNameAsync(_adminOpt.UserName);
        if (admin is null)
        {
            admin = new ApplicationUser
            {
                UserName = _adminOpt.UserName,
                DisplayName = _adminOpt.DisplayName ?? "Administrator",
                IsActive = true
            };

            var created = await userManager.CreateAsync(admin, _adminOpt.Password);
            if (!created.Succeeded)
                throw new InvalidOperationException("Bootstrap admin create failed: " +
                                                    string.Join("; ", created.Errors.Select(e => e.Description)));
        }

        if (!await userManager.IsInRoleAsync(admin, AppRoles.Admin))
        {
            var added = await userManager.AddToRoleAsync(admin, AppRoles.Admin);
            if (!added.Succeeded)
                throw new InvalidOperationException("Bootstrap admin role add failed: " +
                                                    string.Join("; ", added.Errors.Select(e => e.Description)));
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
