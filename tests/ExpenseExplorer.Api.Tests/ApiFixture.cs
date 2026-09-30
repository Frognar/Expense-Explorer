using System.Net.Http.Headers;
using System.Net.Http.Json;
using ExpenseExplorer.Api.Auth;
using ExpenseExplorer.Contracts.Auth;
using ExpenseExplorer.Domain.Users;
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
    private string _editorToken = "";
    private string _readerToken = "";

    public ApiFactory App => _default ?? throw new InvalidOperationException("Fixture is not initialized.");

    /// <summary>A client signed in as a user who can change data.</summary>
    public HttpClient Editor => App.CreateClient().WithAccessToken(_editorToken);

    /// <summary>A client signed in as a user who can only read.</summary>
    public HttpClient Reader => App.CreateClient().WithAccessToken(_readerToken);

    public HttpClient Anonymous => App.CreateClient();

    public async ValueTask InitializeAsync()
    {
        await _postgres.StartAsync();
        _default = new ApiFactory(await CreateDatabaseAsync("api_tests"));
        _editorToken = await _default.SignInAsync(UserRole.Editor);
        _readerToken = await _default.SignInAsync(UserRole.Reader);
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

public sealed class ApiFactory(string connectionString, int signInAttemptsPerMinute = 1000) : WebApplicationFactory<Program>
{
    public const string TestPassword = "correct horse battery";

    private readonly string _signingKeyPath = Path.Combine(Path.GetTempPath(), $"expense-explorer-{Guid.NewGuid()}.key");

    /// <summary>Runs <c>users ...</c> of the command line; returns the exit code and what it printed.</summary>
    public async Task<(int ExitCode, string Output)> RunUsersAsync(string password, params string[] args)
    {
        await using StringWriter output = new();
        int exitCode = await UserCommandLine.RunAsync(Services, ["users", .. args], () => password, output, CancellationToken.None);
        return (exitCode, output.ToString());
    }

    /// <summary>Creates the user of this role if needed and returns an access token for it.</summary>
    public async Task<string> SignInAsync(UserRole role)
    {
        string userName = role == UserRole.Editor ? "editor" : "reader";
        await RunUsersAsync(TestPassword, "add", userName, userName);

        HttpResponseMessage response = await CreateClient().PostAsJsonAsync(
            new Uri("/api/v1/auth/login", UriKind.Relative),
            new LoginRequest(userName, TestPassword));
        response.EnsureSuccessStatusCode();
        SessionResponse session = await response.Content.ReadFromJsonAsync<SessionResponse>()
            ?? throw new InvalidOperationException("Sign-in returned no session.");
        return session.AccessToken;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:expense-explorer", connectionString);
        builder.UseSetting("Auth:SigningKeyPath", _signingKeyPath);
        builder.UseSetting("Auth:SignInAttemptsPerMinute", signInAttemptsPerMinute.ToString(System.Globalization.CultureInfo.InvariantCulture));
        builder.ConfigureTestServices(services => services.AddSingleton<TimeProvider>(new FixedClock(ApiFixture.Today)));
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        File.Delete(_signingKeyPath);
    }

    private sealed class FixedClock(DateOnly today) : TimeProvider
    {
        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;

        public override DateTimeOffset GetUtcNow() => new(today.ToDateTime(new TimeOnly(12, 0)), TimeSpan.Zero);
    }
}

internal static class ClientExtensions
{
    public static HttpClient WithAccessToken(this HttpClient client, string accessToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }
}
