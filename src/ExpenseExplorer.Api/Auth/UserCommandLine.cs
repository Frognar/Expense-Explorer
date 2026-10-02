using System.Text;
using ExpenseExplorer.Application.Users;
using ExpenseExplorer.Domain.Common;
using ExpenseExplorer.Domain.Users;

namespace ExpenseExplorer.Api.Auth;

/// <summary>
/// Account management without any admin UI, e.g.
/// <c>docker exec -it expense-explorer-api dotnet ExpenseExplorer.Api.dll users add jan editor</c>.
/// Passwords are always typed at a prompt, never passed as arguments, so they stay out of shell history.
/// </summary>
internal static class UserCommandLine
{
    private const string Usage = """
        Usage:
          users list
          users add <name> <reader|editor|admin>   asks for the password
          users password <name>                    asks for the new password; signs the user out everywhere
          users role <name> <reader|editor|admin>
          users remove <name>
        """;

    public static bool IsInvoked(string[] args) => args is ["users", ..];

    public static async Task<int> RunAsync(
        IServiceProvider services,
        string[] args,
        Func<string?> readPassword,
        TextWriter output,
        CancellationToken cancellationToken)
    {
        await using AsyncServiceScope scope = services.CreateAsyncScope();
        IUserAccounts accounts = scope.ServiceProvider.GetRequiredService<IUserAccounts>();
        IRefreshTokens refreshTokens = scope.ServiceProvider.GetRequiredService<IRefreshTokens>();
        DateTimeOffset now = scope.ServiceProvider.GetRequiredService<TimeProvider>().GetUtcNow();

        switch (args)
        {
            case ["users", "list"]:
                IReadOnlyList<AuthenticatedUser> users = await accounts.ListAsync(cancellationToken);
                foreach (AuthenticatedUser user in users)
                {
                    await output.WriteLineAsync($"{user.UserName}\t{user.Role}");
                }

                return 0;
            case ["users", "add", var name, var role]:
                return await ReportAsync(
                    output,
                    "created",
                    await RunAsync(
                        ResultCombine.Combine(
                            UserName.Create(name), Password.Create(readPassword()), ParseRole(role),
                            (userName, password, userRole) => new CreateUser(userName, password, userRole)),
                        command => UserAdministration.CreateAsync(accounts, command, cancellationToken)));
            case ["users", "password", var name]:
                return await ReportAsync(
                    output,
                    "has a new password",
                    await RunAsync(
                        ResultCombine.Combine(
                            UserName.Create(name), Password.Create(readPassword()),
                            (userName, password) => new ChangePassword(userName, password)),
                        command => UserAdministration.ChangePasswordAsync(accounts, refreshTokens, command, now, cancellationToken)));
            case ["users", "role", var name, var role]:
                return await ReportAsync(
                    output,
                    "has a new role",
                    await RunAsync(
                        ResultCombine.Combine(
                            UserName.Create(name), ParseRole(role),
                            (userName, userRole) => new ChangeRole(userName, userRole)),
                        command => UserAdministration.ChangeRoleAsync(accounts, command, cancellationToken)));
            case ["users", "remove", var name]:
                return await ReportAsync(
                    output,
                    "removed",
                    await RunAsync(
                        UserName.Create(name).Map(userName => new RemoveUser(userName)),
                        command => UserAdministration.RemoveAsync(accounts, command, cancellationToken)));
            default:
                await output.WriteLineAsync(Usage);
                return 2;
        }
    }

    /// <summary>Reads a password from the terminal without echoing it, or a line from redirected input.</summary>
    public static string? ReadPasswordFromConsole()
    {
        Console.Write("Password: ");
        if (Console.IsInputRedirected)
        {
            return Console.ReadLine();
        }

        StringBuilder password = new();
        for (ConsoleKeyInfo key = Console.ReadKey(intercept: true); key.Key != ConsoleKey.Enter; key = Console.ReadKey(intercept: true))
        {
            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password.Length--;
            }
            else if (!char.IsControl(key.KeyChar))
            {
                password.Append(key.KeyChar);
            }
        }

        Console.WriteLine();
        return password.ToString();
    }

    private static Result<UserRole> ParseRole(string value) =>
        Enum.TryParse(value, ignoreCase: true, out UserRole role) && Enum.IsDefined(role)
            ? Result.Success(role)
            : Result.Failure<UserRole>(new Error("UserRole.Unknown", "Role must be 'reader', 'editor' or 'admin'."));

    private static Task<Result<AuthenticatedUser>> RunAsync<TCommand>(
        Result<TCommand> command,
        Func<TCommand, Task<Result<AuthenticatedUser>>> run) =>
        command.Match(run, errors => Task.FromResult(Result.Failure<AuthenticatedUser>(errors)));

    private static async Task<int> ReportAsync(TextWriter output, string outcome, Result<AuthenticatedUser> result) =>
        await result.Match(
            async user =>
            {
                await output.WriteLineAsync($"User '{user.UserName}' ({user.Role}) {outcome}.");
                return 0;
            },
            async errors =>
            {
                foreach (Error error in errors)
                {
                    await output.WriteLineAsync($"Error: {error.Message}");
                }

                return 1;
            });
}
