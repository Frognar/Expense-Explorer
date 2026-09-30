using Microsoft.AspNetCore.Identity;

namespace ExpenseExplorer.Infrastructure.Users;

/// <summary>Stored account. ASP.NET Core Identity takes care of password hashing and lockout.</summary>
internal sealed class UserAccount : IdentityUser<Guid>;
