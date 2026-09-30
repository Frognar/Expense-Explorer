using System.Diagnostics.CodeAnalysis;
using ExpenseExplorer.Application.Users;

namespace ExpenseExplorer.Api.Auth;

/// <summary>
/// The refresh token lives in an HttpOnly cookie sent only to the auth endpoints, so scripts on
/// the page cannot steal it and other requests do not carry it.
/// </summary>
internal static class RefreshCookie
{
    public const string Name = "ee_refresh";

    public static string? Read(HttpRequest request) => request.Cookies[Name];

    public static void Write(HttpResponse response, RefreshToken token) =>
        response.Cookies.Append(Name, token.Value, Options(response.HttpContext.Request, token.ExpiresAt));

    public static void Delete(HttpResponse response) =>
        response.Cookies.Delete(Name, Options(response.HttpContext.Request, expiresAt: null));

    [SuppressMessage(
        "Security",
        "S2092:Cookies should be \"secure\"",
        Justification = "Secure whenever the request came over HTTPS; the app may also be served over plain HTTP on a home network.")]
    private static CookieOptions Options(HttpRequest request, DateTimeOffset? expiresAt) =>
        new()
        {
            HttpOnly = true,
            Secure = request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Path = $"{Routing.ApiPrefix}/auth",
            Expires = expiresAt,
        };
}
