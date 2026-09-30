using ExpenseExplorer.Application.Users;
using ExpenseExplorer.Domain.Common;
using ExpenseExplorer.Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExpenseExplorer.Infrastructure.Users;

internal sealed class IdentityUserAccounts(UserManager<UserAccount> users) : IUserAccounts
{
    public async Task<AuthenticatedUser?> VerifyAsync(string userName, string password, CancellationToken cancellationToken)
    {
        UserAccount? account = await users.FindByNameAsync(userName);
        if (account is null || await users.IsLockedOutAsync(account))
        {
            return null;
        }

        if (!await users.CheckPasswordAsync(account, password))
        {
            await users.AccessFailedAsync(account);
            return null;
        }

        await users.ResetAccessFailedCountAsync(account);
        return await ToUserAsync(account);
    }

    public async Task<AuthenticatedUser?> FindAsync(Guid id, CancellationToken cancellationToken) =>
        await users.FindByIdAsync(id.ToString()) is { } account
            ? await ToUserAsync(account)
            : null;

    public async Task<IReadOnlyList<AuthenticatedUser>> ListAsync(CancellationToken cancellationToken)
    {
        List<UserAccount> accounts = await users.Users.OrderBy(account => account.UserName).ToListAsync(cancellationToken);
        List<AuthenticatedUser> result = [];
        foreach (UserAccount account in accounts)
        {
            result.Add(await ToUserAsync(account));
        }

        return result;
    }

    public async Task<Result<AuthenticatedUser>> CreateAsync(
        UserName userName,
        Password password,
        UserRole role,
        CancellationToken cancellationToken)
    {
        if (await users.FindByNameAsync(userName.Value) is not null)
        {
            return Result.Failure<AuthenticatedUser>(UserErrors.UserNameTaken);
        }

        UserAccount account = new() { Id = Guid.CreateVersion7(), UserName = userName.Value };
        return await ThenAsync(
            await users.CreateAsync(account, password.Value),
            () => users.AddToRoleAsync(account, IdentityRoles.NameOf(role)),
            account);
    }

    public Task<Result<AuthenticatedUser>> ChangePasswordAsync(UserName userName, Password password, CancellationToken cancellationToken) =>
        WithAccountAsync(userName, async account =>
            await ThenAsync(
                await users.RemovePasswordAsync(account),
                () => users.AddPasswordAsync(account, password.Value)));

    public Task<Result<AuthenticatedUser>> ChangeRoleAsync(UserName userName, UserRole role, CancellationToken cancellationToken) =>
        WithAccountAsync(userName, async account =>
            await ThenAsync(
                await users.RemoveFromRolesAsync(account, await users.GetRolesAsync(account)),
                () => users.AddToRoleAsync(account, IdentityRoles.NameOf(role))));

    public async Task<Result<AuthenticatedUser>> RemoveAsync(UserName userName, CancellationToken cancellationToken)
    {
        if (await users.FindByNameAsync(userName.Value) is not { } account)
        {
            return Result.Failure<AuthenticatedUser>(UserErrors.UserNotFound);
        }

        AuthenticatedUser removed = await ToUserAsync(account);
        return ToResult(await users.DeleteAsync(account), removed);
    }

    private async Task<Result<AuthenticatedUser>> WithAccountAsync(
        UserName userName,
        Func<UserAccount, Task<IdentityResult>> change)
    {
        if (await users.FindByNameAsync(userName.Value) is not { } account)
        {
            return Result.Failure<AuthenticatedUser>(UserErrors.UserNotFound);
        }

        IdentityResult changed = await change(account);
        return ToResult(changed, await ToUserAsync(account));
    }

    private async Task<Result<AuthenticatedUser>> ThenAsync(
        IdentityResult first,
        Func<Task<IdentityResult>> second,
        UserAccount account) =>
        ToResult(await ThenAsync(first, second), await ToUserAsync(account));

    private static async Task<IdentityResult> ThenAsync(IdentityResult first, Func<Task<IdentityResult>> second) =>
        first.Succeeded ? await second() : first;

    private async Task<AuthenticatedUser> ToUserAsync(UserAccount account) =>
        new(
            account.Id,
            account.UserName ?? "",
            IdentityRoles.FromNames(await users.GetRolesAsync(account)) ?? UserRole.Reader);

    private static Result<AuthenticatedUser> ToResult(IdentityResult identityResult, AuthenticatedUser user) =>
        identityResult.Succeeded
            ? Result.Success(user)
            : Result.Failure<AuthenticatedUser>(
                [.. identityResult.Errors.Select(error => new Error($"Identity.{error.Code}", error.Description))]);
}
