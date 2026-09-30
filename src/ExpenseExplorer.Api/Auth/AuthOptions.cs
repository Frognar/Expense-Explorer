namespace ExpenseExplorer.Api.Auth;

/// <summary>The "Auth" section of appsettings.</summary>
internal sealed class AuthOptions
{
    public const string Section = "Auth";

    public string Issuer { get; set; } = "expense-explorer";

    public string Audience { get; set; } = "expense-explorer";

    public TimeSpan AccessTokenLifetime { get; set; } = TimeSpan.FromMinutes(15);

    public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(30);

    /// <summary>Where the key signing access tokens is kept. It is created on first start.</summary>
    public string SigningKeyPath { get; set; } = "keys/jwt-signing.key";

    /// <summary>Sign-in attempts allowed per client address and minute.</summary>
    public int SignInAttemptsPerMinute { get; set; } = 10;
}
