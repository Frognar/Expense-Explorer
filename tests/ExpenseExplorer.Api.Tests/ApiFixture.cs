using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Testcontainers.PostgreSql;

[assembly: AssemblyFixture(typeof(ExpenseExplorer.Api.Tests.ApiFixture))]

namespace ExpenseExplorer.Api.Tests;

/// <summary>One PostgreSQL container for the whole test run; each API instance gets its own database.</summary>
public sealed class ApiFixture : IAsyncLifetime
{
    public static readonly DateOnly Today = new(2026, 9, 29);

    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16").Build();

    private ApiFactory? _default;

    public HttpClient Client => (_default ?? throw new InvalidOperationException("Fixture is not initialized.")).CreateClient();

    public async ValueTask InitializeAsync()
    {
        await _postgres.StartAsync();
        _default = new ApiFactory(await CreateDatabaseAsync("api_tests"));
    }

    public async ValueTask DisposeAsync()
    {
        if (_default is not null)
        {
            await _default.DisposeAsync();
        }

        await _postgres.DisposeAsync();
    }

    /// <summary>Creates an empty database and returns its connection string.</summary>
    public async Task<string> CreateDatabaseAsync(string name)
    {
        await using NpgsqlConnection connection = new(_postgres.GetConnectionString());
        await connection.OpenAsync();
        await using NpgsqlCommand command = new($"create database {name}", connection);
        await command.ExecuteNonQueryAsync();

        return new NpgsqlConnectionStringBuilder(_postgres.GetConnectionString()) { Database = name }.ConnectionString;
    }
}

public sealed class ApiFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:expense-explorer", connectionString);
        builder.ConfigureTestServices(services => services.AddSingleton<TimeProvider>(new FixedClock(ApiFixture.Today)));
    }

    private sealed class FixedClock(DateOnly today) : TimeProvider
    {
        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;

        public override DateTimeOffset GetUtcNow() => new(today.ToDateTime(new TimeOnly(12, 0)), TimeSpan.Zero);
    }
}
