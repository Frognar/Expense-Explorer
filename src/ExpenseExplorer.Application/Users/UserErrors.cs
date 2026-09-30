using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Application.Users;

public static class UserErrors
{
    /// <summary>Same answer for an unknown user, a wrong password and a locked account, so none can be told apart.</summary>
    public static readonly Error InvalidCredentials = new(
        "Auth.InvalidCredentials", "User name or password is incorrect.", ErrorType.Unauthorized);

    public static readonly Error SessionExpired = new(
        "Auth.SessionExpired", "The session has expired. Sign in again.", ErrorType.Unauthorized);

    public static readonly Error UserNameTaken = new(
        "User.NameTaken", "A user with this name already exists.", ErrorType.Conflict);

    public static readonly Error UserNotFound = new(
        "User.NotFound", "User was not found.", ErrorType.NotFound);
}
