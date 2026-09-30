using ExpenseExplorer.Api.Http;
using ExpenseExplorer.Application.Users;
using ExpenseExplorer.Contracts.Auth;
using ExpenseExplorer.Domain.Common;
using ExpenseExplorer.Domain.Users;
using Microsoft.Extensions.Options;

namespace ExpenseExplorer.Api.Auth;

internal static class AuthEndpoints
{
    public const string SignInRateLimit = "sign-in";

    public static RouteGroupBuilder MapAuth(this RouteGroupBuilder api)
    {
        RouteGroupBuilder auth = api.MapGroup("/auth").WithTags("Auth");

        auth.MapPost("/login", LoginAsync).RequireRateLimiting(SignInRateLimit);
        auth.MapPost("/refresh", RefreshAsync);
        auth.MapPost("/logout", LogoutAsync);
        auth.MapGet("/me", Me).RequireAuthorization(Policies.CanRead);

        return api;
    }

    private static Task<IResult> LoginAsync(
        LoginRequest request,
        HttpResponse response,
        IUserAccounts accounts,
        IRefreshTokens refreshTokens,
        SessionIssuer sessions,
        CancellationToken cancellationToken) =>
        ParseLogin(request).ToHttpAsync(
            credentials => SessionUseCases.SignInAsync(accounts, refreshTokens, credentials, sessions.Clock(), cancellationToken),
            session => sessions.Respond(response, session));

    private static async Task<IResult> RefreshAsync(
        HttpRequest request,
        HttpResponse response,
        IUserAccounts accounts,
        IRefreshTokens refreshTokens,
        SessionIssuer sessions,
        CancellationToken cancellationToken)
    {
        Result<Session> session = await SessionUseCases.RefreshAsync(
            accounts, refreshTokens, RefreshCookie.Read(request), sessions.Clock(), cancellationToken);

        return session.Match(
            started => sessions.Respond(response, started),
            errors =>
            {
                RefreshCookie.Delete(response);
                return ErrorResults.From(errors);
            });
    }

    private static async Task<IResult> LogoutAsync(
        HttpRequest request,
        HttpResponse response,
        IRefreshTokens refreshTokens,
        TimeProvider clock,
        CancellationToken cancellationToken)
    {
        await SessionUseCases.SignOutAsync(refreshTokens, RefreshCookie.Read(request), clock.GetUtcNow(), cancellationToken);
        RefreshCookie.Delete(response);
        return Results.NoContent();
    }

    private static IResult Me(HttpContext context) =>
        Results.Ok(ToResponse(
            context.User.Identity?.Name ?? "",
            Enum.Parse<UserRole>(context.User.FindFirst(AccessTokens.RoleClaim)?.Value ?? nameof(UserRole.Reader))));

    private static Result<Credentials> ParseLogin(LoginRequest request) =>
        ResultCombine.Combine(
            Input.Required(request.UserName).ForTarget("userName"),
            Input.Required(request.Password).ForTarget("password"),
            (userName, password) => new Credentials(userName.Trim(), password));

    internal static CurrentUserResponse ToResponse(string userName, UserRole role) =>
        new(userName, role.ToString(), role == UserRole.Editor);
}

/// <summary>Turns a started session into the response: the access token in the body, the refresh token in the cookie.</summary>
internal sealed class SessionIssuer(IOptions<AuthOptions> options, SigningKey key, TimeProvider clock)
{
    public SessionClock Clock() => new(clock.GetUtcNow(), options.Value.RefreshTokenLifetime);

    public IResult Respond(HttpResponse response, Session session)
    {
        AccessToken accessToken = AccessTokens.Create(session.User, clock.GetUtcNow(), options.Value, key);
        RefreshCookie.Write(response, session.RefreshToken);
        return Results.Ok(new SessionResponse(
            accessToken.Value,
            accessToken.ExpiresAt,
            AuthEndpoints.ToResponse(session.User.UserName, session.User.Role)));
    }
}
