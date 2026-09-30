using System.Net;
using ExpenseExplorer.Contracts.Auth;
using ExpenseExplorer.Contracts.Receipts;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ExpenseExplorer.Api.Tests;

public class AuthTests(ApiFixture api)
{
    private const string Login = "/api/v1/auth/login";
    private const string Refresh = "/api/v1/auth/refresh";
    private const string Logout = "/api/v1/auth/logout";
    private const string Me = "/api/v1/auth/me";
    private const string Receipts = "/api/v1/receipts";
    private const string Password = ApiFactory.TestPassword;

    [Fact]
    public async Task Sign_in_returns_an_access_token_and_sets_the_refresh_cookie()
    {
        string user = await CreateUserAsync("editor");

        HttpResponseMessage response = await NewClient().Post(Login, new LoginRequest(user, Password));
        SessionResponse session = await response.Read<SessionResponse>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(session.AccessToken);
        Assert.Equal(new DateTimeOffset(ApiFixture.Today.ToDateTime(new TimeOnly(12, 15)), TimeSpan.Zero), session.ExpiresAt);
        Assert.Equal(new CurrentUserResponse(user, "Editor", CanEdit: true), session.User);
        string cookie = Assert.Single(response.Headers.GetValues("Set-Cookie"));
        Assert.StartsWith("ee_refresh=", cookie, StringComparison.Ordinal);
        Assert.Contains("path=/api/v1/auth", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("samesite=strict", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("httponly", cookie, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Wrong_password_and_unknown_user_get_the_same_answer()
    {
        string user = await CreateUserAsync("reader");

        HttpResponseMessage wrongPassword = await NewClient().Post(Login, new LoginRequest(user, "not the password"));
        HttpResponseMessage unknownUser = await NewClient().Post(Login, new LoginRequest("nobody", Password));

        Assert.Equal(HttpStatusCode.Unauthorized, wrongPassword.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, unknownUser.StatusCode);
        Assert.Equal(["Auth.InvalidCredentials"], (await wrongPassword.ErrorCodes())[""]);
        Assert.Equal(["Auth.InvalidCredentials"], (await unknownUser.ErrorCodes())[""]);
    }

    [Fact]
    public async Task Sign_in_reports_missing_fields()
    {
        HttpResponseMessage response = await NewClient().Post(Login, new LoginRequest(" ", null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(
            new Dictionary<string, string[]>
            {
                ["userName"] = ["Input.Required"],
                ["password"] = ["Input.Required"],
            },
            await response.ErrorCodes());
    }

    [Fact]
    public async Task Five_wrong_passwords_lock_the_account()
    {
        string user = await CreateUserAsync("editor");
        for (int attempt = 0; attempt < 5; attempt++)
        {
            await NewClient().Post(Login, new LoginRequest(user, "not the password"));
        }

        HttpResponseMessage response = await NewClient().Post(Login, new LoginRequest(user, Password));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Refresh_rotates_the_token_and_a_reused_token_ends_every_session()
    {
        string user = await CreateUserAsync("editor");
        string first = RefreshCookieOf(await NewClient().Post(Login, new LoginRequest(user, Password)));

        HttpResponseMessage refreshed = await PostWithCookie(Refresh, first);
        string second = RefreshCookieOf(refreshed);
        HttpResponseMessage reused = await PostWithCookie(Refresh, first);
        HttpResponseMessage afterReuse = await PostWithCookie(Refresh, second);

        Assert.Equal(HttpStatusCode.OK, refreshed.StatusCode);
        Assert.NotEqual(first, second);
        Assert.Equal(HttpStatusCode.Unauthorized, reused.StatusCode);
        Assert.Equal(["Auth.SessionExpired"], (await reused.ErrorCodes())[""]);
        Assert.Equal(HttpStatusCode.Unauthorized, afterReuse.StatusCode);
    }

    [Fact]
    public async Task Refresh_without_a_cookie_is_unauthorized()
    {
        HttpResponseMessage response = await NewClient().PostAsync(new Uri(Refresh, UriKind.Relative), null, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Sign_out_ends_the_session()
    {
        string user = await CreateUserAsync("reader");
        string cookie = RefreshCookieOf(await NewClient().Post(Login, new LoginRequest(user, Password)));

        HttpResponseMessage signedOut = await PostWithCookie(Logout, cookie);
        HttpResponseMessage refreshed = await PostWithCookie(Refresh, cookie);

        Assert.Equal(HttpStatusCode.NoContent, signedOut.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, refreshed.StatusCode);
    }

    [Fact]
    public async Task New_password_ends_existing_sessions()
    {
        string user = await CreateUserAsync("editor");
        string cookie = RefreshCookieOf(await NewClient().Post(Login, new LoginRequest(user, Password)));

        (int exitCode, _) = await api.App.RunUsersAsync("a brand new passphrase", "password", user);
        HttpResponseMessage refreshed = await PostWithCookie(Refresh, cookie);
        HttpResponseMessage withNewPassword = await NewClient().Post(Login, new LoginRequest(user, "a brand new passphrase"));

        Assert.Equal(0, exitCode);
        Assert.Equal(HttpStatusCode.Unauthorized, refreshed.StatusCode);
        Assert.Equal(HttpStatusCode.OK, withNewPassword.StatusCode);
    }

    [Fact]
    public async Task Changed_role_applies_at_the_next_refresh()
    {
        string user = await CreateUserAsync("editor");
        string cookie = RefreshCookieOf(await NewClient().Post(Login, new LoginRequest(user, Password)));

        await api.App.RunUsersAsync("", "role", user, "reader");
        SessionResponse session = await (await PostWithCookie(Refresh, cookie)).Read<SessionResponse>();

        Assert.Equal(new CurrentUserResponse(user, "Reader", CanEdit: false), session.User);
    }

    [Fact]
    public async Task Me_describes_the_signed_in_user()
    {
        HttpResponseMessage response = await api.Reader.Get(Me);

        Assert.Equal(new CurrentUserResponse("reader", "Reader", CanEdit: false), await response.Read<CurrentUserResponse>());
    }

    [Fact]
    public async Task Data_needs_a_signed_in_user()
    {
        HttpResponseMessage response = await api.Anonymous.Get(Receipts);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task A_forged_token_is_rejected()
    {
        HttpResponseMessage response = await api.Anonymous.WithAccessToken("eyJhbGciOiJub25lIn0.eyJzdWIiOiJ4In0.").Get(Receipts);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Reader_can_browse_but_not_change()
    {
        HttpResponseMessage list = await api.Reader.Get(Receipts);
        HttpResponseMessage create = await api.Reader.Post(Receipts, new CreateReceiptRequest("Lidl", ApiFixture.Today));

        Assert.Equal(HttpStatusCode.OK, list.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, create.StatusCode);
        Assert.Equal("application/problem+json", create.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Too_many_sign_in_attempts_are_throttled()
    {
        await using ApiFactory app = new(await api.CreateDatabaseAsync("sign_in_limit"), signInAttemptsPerMinute: 2);
        HttpClient client = app.CreateClient();

        HttpStatusCode[] statuses = await Task.WhenAll(Enumerable.Range(0, 3).Select(async _ =>
            (await client.Post(Login, new LoginRequest("nobody", Password))).StatusCode));

        Assert.Equal(1, statuses.Count(status => status == HttpStatusCode.TooManyRequests));
    }

    [Theory]
    [InlineData("add", "x", "editor", "User name must have 3 to 50 characters.")]
    [InlineData("add", "valid.name", "admin", "Role must be 'reader' or 'editor'.")]
    [InlineData("password", "nobody", null, "User was not found.")]
    public async Task Command_line_reports_invalid_input(string command, string name, string? role, string message)
    {
        (int exitCode, string output) = await api.App.RunUsersAsync(Password, role is null ? [command, name] : [command, name, role]);

        Assert.Equal(1, exitCode);
        Assert.Contains(message, output, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Command_line_rejects_a_short_password_and_a_taken_name()
    {
        string user = await CreateUserAsync("reader");

        (int shortPassword, string shortOutput) = await api.App.RunUsersAsync("short", "add", $"{user}2", "reader");
        (int taken, string takenOutput) = await api.App.RunUsersAsync(Password, "add", user, "reader");

        Assert.Equal(1, shortPassword);
        Assert.Contains("at least 10 characters", shortOutput, StringComparison.Ordinal);
        Assert.Equal(1, taken);
        Assert.Contains("already exists", takenOutput, StringComparison.Ordinal);
    }

    private async Task<string> CreateUserAsync(string role)
    {
        string user = $"user-{Guid.NewGuid():N}"[..20];
        (int exitCode, string output) = await api.App.RunUsersAsync(Password, "add", user, role);
        Assert.True(exitCode == 0, output);
        return user;
    }

    private HttpClient NewClient() => api.App.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });

    private async Task<HttpResponseMessage> PostWithCookie(string url, string refreshCookie)
    {
        using HttpRequestMessage request = new(HttpMethod.Post, new Uri(url, UriKind.Relative));
        request.Headers.Add("Cookie", refreshCookie);
        return await NewClient().SendAsync(request, TestContext.Current.CancellationToken);
    }

    /// <summary>The "name=value" part of the refresh cookie a response sets.</summary>
    private static string RefreshCookieOf(HttpResponseMessage response) =>
        response.Headers.GetValues("Set-Cookie").Single().Split(';')[0];
}
