using GIPractice.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GIPractice.Api.Infrastructure;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var loggerFactory = services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("IdentitySeeder");

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

        string[] roles = ["Admin", "Doctor", "Reception"];

        // Ensure roles
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new ApplicationRole { Name = roleName };
                var result = await roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    logger.LogError("Failed to create role {Role}: {Errors}",
                        roleName,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
                else
                {
                    logger.LogInformation("Created role {Role}", roleName);
                }
            }
        }

        // Default admin
        const string adminUserName = "admin";
        const string adminEmail = "admin@example.com";
        // TODO: change this in production
        const string adminPassword = "Admin123!";

        var admin = await userManager.FindByNameAsync(adminUserName);
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = adminUserName,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(admin, adminPassword);
            if (!createResult.Succeeded)
            {
                logger.LogError("Failed to create default admin user: {Errors}",
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
                return;
            }

            logger.LogInformation("Created default admin user {User}", adminUserName);
        }

        // Ensure admin is in Admin role
        var rolesForAdmin = await userManager.GetRolesAsync(admin);
        if (!rolesForAdmin.Contains("Admin"))
        {
            var result = await userManager.AddToRoleAsync(admin, "Admin");
            if (!result.Succeeded)
            {
                logger.LogError("Failed to add admin user to Admin role: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            else
            {
                logger.LogInformation("Added {User} to Admin role", adminUserName);
            }
        }
    }
}
