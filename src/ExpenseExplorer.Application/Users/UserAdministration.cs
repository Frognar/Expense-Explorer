using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Application.Users;

/// <summary>Account management, run by the administrator from the command line.</summary>
public static class UserAdministration
{
    public static Task<Result<AuthenticatedUser>> CreateAsync(
        IUserAccounts accounts,
        CreateUser command,
        CancellationToken cancellationToken) =>
        accounts.CreateAsync(command.UserName, command.Password, command.Role, cancellationToken);

    /// <summary>A new password also ends every session started with the old one.</summary>
    public static async Task<Result<AuthenticatedUser>> ChangePasswordAsync(
        IUserAccounts accounts,
        IRefreshTokens refreshTokens,
        ChangePassword command,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        Result<AuthenticatedUser> result = await accounts.ChangePasswordAsync(command.UserName, command.Password, cancellationToken);
        await result.Match(
            user => refreshTokens.RevokeAllAsync(user.Id, now, cancellationToken),
            _ => Task.CompletedTask);
        return result;
    }

    public static Task<Result<AuthenticatedUser>> ChangeRoleAsync(
        IUserAccounts accounts,
        ChangeRole command,
        CancellationToken cancellationToken) =>
        accounts.ChangeRoleAsync(command.UserName, command.Role, cancellationToken);

    public static Task<Result<AuthenticatedUser>> RemoveAsync(
        IUserAccounts accounts,
        RemoveUser command,
        CancellationToken cancellationToken) =>
        accounts.RemoveAsync(command.UserName, cancellationToken);
}
