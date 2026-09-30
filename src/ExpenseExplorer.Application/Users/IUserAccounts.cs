using ExpenseExplorer.Domain.Common;
using ExpenseExplorer.Domain.Users;

namespace ExpenseExplorer.Application.Users;

/// <summary>Stored accounts. Passwords never leave the store, they are only verified.</summary>
public interface IUserAccounts
{
    /// <summary>
    /// The account, when the password matches. Repeated failures lock the account for a while,
    /// and a locked account is not verified even with the right password.
    /// </summary>
    Task<AuthenticatedUser?> VerifyAsync(string userName, string password, CancellationToken cancellationToken);

    Task<AuthenticatedUser?> FindAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyList<AuthenticatedUser>> ListAsync(CancellationToken cancellationToken);

    Task<Result<AuthenticatedUser>> CreateAsync(UserName userName, Password password, UserRole role, CancellationToken cancellationToken);

    Task<Result<AuthenticatedUser>> ChangePasswordAsync(UserName userName, Password password, CancellationToken cancellationToken);

    Task<Result<AuthenticatedUser>> ChangeRoleAsync(UserName userName, UserRole role, CancellationToken cancellationToken);

    Task<Result<AuthenticatedUser>> RemoveAsync(UserName userName, CancellationToken cancellationToken);
}
