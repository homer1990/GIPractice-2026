using GIPractice.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GIPractice.Api.Hosting;

public static class DatabaseCli
{
    public static async Task<bool> TryRunAsync(
        string[] args,
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        var command = args.FirstOrDefault()?.Trim().ToLowerInvariant();
        if (command is not ("db-check" or "db-migrate" or "db-pending"))
            return false;

        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        switch (command)
        {
            case "db-check":
            {
                var canConnect = await db.Database.CanConnectAsync(cancellationToken);
                Console.WriteLine(canConnect ? "Database connection: OK" : "Database connection: FAILED");
                Environment.ExitCode = canConnect ? 0 : 2;
                return true;
            }

            case "db-pending":
            {
                var pending = (await db.Database.GetPendingMigrationsAsync(cancellationToken)).ToArray();
                if (pending.Length == 0)
                {
                    Console.WriteLine("No pending migrations.");
                    return true;
                }

                Console.WriteLine("Pending migrations:");
                foreach (var migration in pending)
                    Console.WriteLine($"  {migration}");

                Environment.ExitCode = 3;
                return true;
            }

            case "db-migrate":
                await db.Database.MigrateAsync(cancellationToken);
                Console.WriteLine("Database migrations applied.");
                return true;

            default:
                return false;
        }
    }
}
