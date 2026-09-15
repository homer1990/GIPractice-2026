using Microsoft.EntityFrameworkCore;

namespace GIPractice.Infrastructure.Database;

public static class DatabaseProviderConfigurator
{
    public static void Configure(
        DbContextOptionsBuilder options,
        string provider,
        string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Database connection string is not configured.");

        switch (Normalize(provider))
        {
            case "sqlserver":
                options.UseSqlServer(connectionString, x => x.MigrationsAssembly("GIPractice.Infrastructure"));
                break;

            case "sqlite":
                options.UseSqlite(connectionString, x => x.MigrationsAssembly("GIPractice.Infrastructure"));
                break;

            case "postgresql":
                options.UseNpgsql(connectionString, x => x.MigrationsAssembly("GIPractice.Infrastructure"));
                break;

            case "mysql":
            case "mariadb":
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    x => x.MigrationsAssembly("GIPractice.Infrastructure"));
                break;

            default:
                throw new InvalidOperationException(
                    $"Unsupported database provider '{provider}'. " +
                    "Supported values: SqlServer, SQLite, PostgreSQL, MySQL, MariaDB.");
        }
    }

    public static string Normalize(string? provider) =>
        (provider ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "mssql" or "sql server" or "sqlserver" => "sqlserver",
            "sqlite" => "sqlite",
            "postgres" or "postgresql" or "npgsql" => "postgresql",
            "mysql" => "mysql",
            "maria" or "mariadb" => "mariadb",
            var value => value
        };
}
