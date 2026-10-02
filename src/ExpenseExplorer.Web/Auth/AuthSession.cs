using System.Net.Http.Json;
using ExpenseExplorer.Contracts.Auth;
using ExpenseExplorer.Web.Api;

namespace ExpenseExplorer.Web.Auth;

/// <summary>
/// The signed-in user and their access token. The token lives only in memory; the refresh token
/// is an HttpOnly cookie the browser sends to the auth endpoints, so a reload restores the session.
/// Refreshes run one at a time: the API ends every session when a refresh token is used twice.
/// </summary>
public sealed class AuthSession(HttpClient http, TimeProvider clock) : IDisposable
{
    private static readonly TimeSpan RefreshAhead = TimeSpan.FromSeconds(30);

    private readonly SemaphoreSlim _refreshing = new(1, 1);
    private SessionResponse? _session;

    public event EventHandler? Changed;

    public bool IsRestored { get; private set; }

    public CurrentUserResponse? User => _session?.User;

    public bool IsSignedIn => _session is not null;

    public bool CanEdit => _session?.User.CanEdit == true;

    public bool CanViewLogs => _session?.User.CanViewLogs == true;

    public async Task<ApiProblem?> SignInAsync(string? userName, string? password)
    {
        using HttpResponseMessage response = await http.PostAsJsonAsync(
            new Uri("api/v1/auth/login", UriKind.Relative),
            new LoginRequest(userName, password));
        if (!response.IsSuccessStatusCode)
        {
            return await ProblemOf(response);
        }

        Start(await response.Content.ReadFromJsonAsync<SessionResponse>());
        return null;
    }

    /// <summary>Called once at start-up: signs the user in again with the refresh cookie, if there is one.</summary>
    public async Task RestoreAsync()
    {
        if (!IsRestored)
        {
            await RefreshAsync(staleToken: null);
            IsRestored = true;
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>A valid access token, refreshed first when it is about to expire; null when signed out.</summary>
    public async Task<string?> AccessTokenAsync()
    {
        SessionResponse? session = _session;
        if (session is not null && session.ExpiresAt - clock.GetUtcNow() < RefreshAhead)
        {
            await RefreshAsync(session.AccessToken);
        }

        return _session?.AccessToken;
    }

    /// <summary>
    /// Gets a new access token unless another caller already replaced <paramref name="staleToken"/>.
    /// Returns whether the user is still signed in.
    /// </summary>
    public async Task<bool> RefreshAsync(string? staleToken)
    {
        await _refreshing.WaitAsync();
        try
        {
            if (_session is not null && _session.AccessToken != staleToken)
            {
                return true;
            }

            using HttpResponseMessage response = await http.PostAsync(new Uri("api/v1/auth/refresh", UriKind.Relative), null);
            Start(response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<SessionResponse>() : null);
            return _session is not null;
        }
        finally
        {
            _refreshing.Release();
        }
    }

    public async Task SignOutAsync()
    {
        using HttpResponseMessage _ = await http.PostAsync(new Uri("api/v1/auth/logout", UriKind.Relative), null);
        Start(null);
    }

    public void Dispose() => _refreshing.Dispose();

    private void Start(SessionResponse? session)
    {
        bool changed = session?.User != _session?.User;
        _session = session;
        if (changed)
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    private static async Task<ApiProblem> ProblemOf(HttpResponseMessage response)
    {
        Dictionary<string, string[]>? codes = null;
        try
        {
            codes = (await response.Content.ReadFromJsonAsync<ProblemBody>())?.ErrorCodes;
        }
        catch (System.Text.Json.JsonException)
        {
            // No ProblemDetails body.
        }

        return new ApiProblem(response.StatusCode, codes ?? new Dictionary<string, string[]> { [""] = [$"Http.{(int)response.StatusCode}"] });
    }

    private sealed record ProblemBody(Dictionary<string, string[]>? ErrorCodes);
}
