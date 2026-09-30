using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Application.Users;

public static class SessionUseCases
{
    public static async Task<Result<Session>> SignInAsync(
        IUserAccounts accounts,
        IRefreshTokens refreshTokens,
        Credentials credentials,
        SessionClock clock,
        CancellationToken cancellationToken)
    {
        AuthenticatedUser? user = await accounts.VerifyAsync(credentials.UserName, credentials.Password, cancellationToken);
        return user is null
            ? Result.Failure<Session>(UserErrors.InvalidCredentials)
            : Result.Success(await StartAsync(refreshTokens, user, clock, cancellationToken));
    }

    /// <summary>Trades a refresh token for a new one. The user is read again, so a changed role applies at once.</summary>
    public static async Task<Result<Session>> RefreshAsync(
        IUserAccounts accounts,
        IRefreshTokens refreshTokens,
        string? refreshToken,
        SessionClock clock,
        CancellationToken cancellationToken)
    {
        Guid? userId = string.IsNullOrEmpty(refreshToken)
            ? null
            : await refreshTokens.RedeemAsync(refreshToken, clock.Now, cancellationToken);
        AuthenticatedUser? user = userId is { } id
            ? await accounts.FindAsync(id, cancellationToken)
            : null;

        return user is null
            ? Result.Failure<Session>(UserErrors.SessionExpired)
            : Result.Success(await StartAsync(refreshTokens, user, clock, cancellationToken));
    }

    public static async Task SignOutAsync(
        IRefreshTokens refreshTokens,
        string? refreshToken,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await refreshTokens.RevokeAsync(refreshToken, now, cancellationToken);
        }
    }

    private static async Task<Session> StartAsync(
        IRefreshTokens refreshTokens,
        AuthenticatedUser user,
        SessionClock clock,
        CancellationToken cancellationToken) =>
        new(user, await refreshTokens.IssueAsync(user.Id, clock.Now, clock.RefreshTokenLifetime, cancellationToken));
}
