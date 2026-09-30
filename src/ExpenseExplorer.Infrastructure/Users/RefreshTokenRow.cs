namespace ExpenseExplorer.Infrastructure.Users;

/// <summary>Only the SHA-256 hash of a refresh token is stored, so a copy of the database cannot be used to sign in.</summary>
internal sealed class RefreshTokenRow
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = "";

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset? RevokedAt { get; set; }
}
