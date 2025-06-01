using System.Data;
using Npgsql;

namespace ExpenseExplorer.Infrastructure.Database;

internal interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken);
}

internal sealed class DbConnectionFactory(string connectionString)
{
    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken)
    {
        NpgsqlConnection connection = new(connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}