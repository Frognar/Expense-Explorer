using ExpenseExplorer.Domain.Users;

namespace ExpenseExplorer.Api.Auth;

internal static class Policies
{
    public const string CanRead = nameof(CanRead);
    public const string CanEdit = nameof(CanEdit);

    public static readonly string[] Readers = [nameof(UserRole.Reader), nameof(UserRole.Editor)];
    public static readonly string[] Editors = [nameof(UserRole.Editor)];
}
