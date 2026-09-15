using LinqToDB;

namespace GIPractice.Infrastructure.Database;

public enum DatabaseProvider
{
    Sqlite = 1,
    PostgreSql = 2,
    MySql = 3,
    MariaDb = 4,
    SqlServer = 5
}

public sealed record DatabaseOptions
{
    public DatabaseProvider Provider { get; }
    public string ConnectionString { get; }

    public DatabaseOptions(DatabaseProvider provider, string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Database connection string is required.", nameof(connectionString));

        Provider = provider;
        ConnectionString = connectionString.Trim();
    }

    public DataOptions CreateLinqToDbOptions()
    {
        var options = new DataOptions();

        return Provider switch
        {
            DatabaseProvider.Sqlite => options.UseSQLite(ConnectionString),
            DatabaseProvider.PostgreSql => options.UsePostgreSQL(ConnectionString),
            DatabaseProvider.MySql or DatabaseProvider.MariaDb => options.UseMySql(ConnectionString),
            DatabaseProvider.SqlServer => options.UseSqlServer(ConnectionString),
            _ => throw new ArgumentOutOfRangeException(nameof(Provider), Provider, null)
        };
    }
}
