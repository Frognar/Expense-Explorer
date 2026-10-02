namespace ExpenseExplorer.Contracts.Auth;

public sealed record LoginRequest(string? UserName, string? Password);

/// <summary>
/// The access token goes in the <c>Authorization: Bearer</c> header of API calls. The refresh token
/// is never in the body: it travels in an HttpOnly cookie that scripts cannot read.
/// </summary>
public sealed record SessionResponse(string AccessToken, DateTimeOffset ExpiresAt, CurrentUserResponse User);

/// <summary>
/// <see cref="Role"/> is <c>Reader</c>, <c>Editor</c> or <c>Admin</c>. Editors and admins can change data;
/// only admins can read the application logs.
/// </summary>
public sealed record CurrentUserResponse(string UserName, string Role, bool CanEdit, bool CanViewLogs);
