namespace ExpenseExplorer.Application.Users;

/// <summary>
/// Long-lived tokens that let a client get new access tokens without the password.
/// Every token can be used once; using it again means it leaked, so all sessions of the user end.
/// </summary>
public interface IRefreshTokens
{
    Task<RefreshToken> IssueAsync(Guid userId, DateTimeOffset now, TimeSpan lifetime, CancellationToken cancellationToken);

    /// <summary>Uses up the token and returns its owner, or null when the token is unknown, used or expired.</summary>
    Task<Guid?> RedeemAsync(string token, DateTimeOffset now, CancellationToken cancellationToken);

    Task RevokeAsync(string token, DateTimeOffset now, CancellationToken cancellationToken);

    Task RevokeAllAsync(Guid userId, DateTimeOffset now, CancellationToken cancellationToken);
}

public sealed record RefreshToken(string Value, DateTimeOffset ExpiresAt);
