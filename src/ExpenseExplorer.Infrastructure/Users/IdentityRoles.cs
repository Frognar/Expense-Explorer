using ExpenseExplorer.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace ExpenseExplorer.Infrastructure.Users;

/// <summary>Identity roles mirror <see cref="UserRole"/>; they are created by a migration, not at runtime.</summary>
internal static class IdentityRoles
{
    public static readonly IReadOnlyList<IdentityRole<Guid>> All =
    [
        Role(UserRole.Reader, "0199a1e2-0000-7000-8000-000000000001"),
        Role(UserRole.Editor, "0199a1e2-0000-7000-8000-000000000002"),
        Role(UserRole.Admin, "0199a1e2-0000-7000-8000-000000000003"),
    ];

    public static string NameOf(UserRole role) => role.ToString();

    /// <summary>The highest role wins if an account was ever given more than one outside the app.</summary>
    public static UserRole? FromNames(IEnumerable<string> names) =>
        Enum.GetValues<UserRole>()
            .Where(role => names.Contains(NameOf(role), StringComparer.Ordinal))
            .Select(role => (UserRole?)role)
            .LastOrDefault();

    private static IdentityRole<Guid> Role(UserRole role, string id) =>
        new()
        {
            Id = Guid.Parse(id),
            Name = NameOf(role),
            NormalizedName = NameOf(role).ToUpperInvariant(),
            ConcurrencyStamp = id,
        };
}
