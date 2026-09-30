using ExpenseExplorer.Domain.Users;

namespace ExpenseExplorer.Application.Users;

public sealed record Credentials(string UserName, string Password);

/// <summary>A signed-in user and the refresh token that keeps the session going.</summary>
public sealed record Session(AuthenticatedUser User, RefreshToken RefreshToken);

/// <summary>When sessions start and how long a refresh token lives.</summary>
public sealed record SessionClock(DateTimeOffset Now, TimeSpan RefreshTokenLifetime);

public sealed record CreateUser(UserName UserName, Password Password, UserRole Role);

public sealed record ChangePassword(UserName UserName, Password Password);

public sealed record ChangeRole(UserName UserName, UserRole Role);

public sealed record RemoveUser(UserName UserName);
