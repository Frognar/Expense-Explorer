using Dapper;
using Npgsql;

namespace AppHost;

internal static class PostgresExtensions
{
    public static IResourceBuilder<PostgresDatabaseResource> CreateDatabase(
        this IResourceBuilder<PostgresServerResource> postgres,
        string dbName)
    {
        const string command = "create-database";
        IResourceBuilder<PostgresDatabaseResource> moviesDb = postgres
            .AddDatabase(dbName)
            .WithCommand(command, "Create Database", async ctx =>
            {
                await CreateDbInNotExists(postgres, dbName, ctx);
                return CommandResults.Success();
            });

        moviesDb.ApplicationBuilder.Eventing.Subscribe<ResourceReadyEvent>(
            moviesDb.Resource.Parent,
            async (ctx, cancel) =>
            {
                await moviesDb
                    .Resource
                    .Annotations
                    .OfType<ResourceCommandAnnotation>()
                    .Single(a => a.Name == command)
                    .ExecuteCommand
                    .Invoke(new ExecuteCommandContext
                    {
                        ServiceProvider = ctx.Services,
                        ResourceName = moviesDb.Resource.Name,
                        CancellationToken = cancel
                    });
            }
        );

        return moviesDb;
    }

    private static async Task CreateDbInNotExists(
        IResourceBuilder<PostgresServerResource> postgres,
        string dbName,
        ExecuteCommandContext ctx)
    {
        string? connectionString = await postgres.Resource
            .ConnectionStringExpression
            .GetValueAsync(ctx.CancellationToken);

        await using NpgsqlConnection connection = new(connectionString);
        await connection.OpenAsync();
        bool? dbExists = await connection
            .QuerySingleOrDefaultAsync<bool>(
                $"SELECT 1 FROM pg_database WHERE datname = '{dbName}'");

        if (dbExists != true)
        {
            await connection.ExecuteAsync($"CREATE DATABASE {dbName}");
        }

        await connection.CloseAsync();
    }
}