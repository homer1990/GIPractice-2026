using FluentMigrator.Runner;
using GIPractice.Infrastructure.Database.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace GIPractice.Infrastructure.Database;

public sealed class DatabaseMigrator(DatabaseOptions options)
{
    public void MigrateUp()
    {
        using var serviceProvider = BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        scope.ServiceProvider.GetRequiredService<IMigrationRunner>().MigrateUp();
    }

    private ServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(builder =>
            {
                var providerBuilder = options.Provider switch
                {
                    DatabaseProvider.Sqlite => builder.AddSQLite(),
                    DatabaseProvider.PostgreSql => builder.AddPostgres(),
                    DatabaseProvider.MySql => builder.AddMySql8(),
                    DatabaseProvider.MariaDb => builder.AddMySql5(),
                    DatabaseProvider.SqlServer => builder.AddSqlServer(),
                    _ => throw new ArgumentOutOfRangeException(nameof(options.Provider), options.Provider, null)
                };

                providerBuilder
                    .WithGlobalConnectionString(options.ConnectionString)
                    .ScanIn(typeof(InitialClinicalCore).Assembly)
                    .For.Migrations();
            });

        return services.BuildServiceProvider(validateScopes: false);
    }
}
