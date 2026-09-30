namespace ExpenseExplorer.Contracts.Auth;

public sealed record LoginRequest(string? UserName, string? Password);

/// <summary>
/// The access token goes in the <c>Authorization: Bearer</c> header of API calls. The refresh token
/// is never in the body: it travels in an HttpOnly cookie that scripts cannot read.
/// </summary>
public sealed record SessionResponse(string AccessToken, DateTimeOffset ExpiresAt, CurrentUserResponse User);

/// <summary><see cref="Role"/> is <c>Reader</c> or <c>Editor</c>; only editors can change data.</summary>
public sealed record CurrentUserResponse(string UserName, string Role, bool CanEdit);
