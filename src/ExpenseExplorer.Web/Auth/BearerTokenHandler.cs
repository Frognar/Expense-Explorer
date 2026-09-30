using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;

namespace ExpenseExplorer.Web.Auth;

/// <summary>
/// Adds the access token to API calls. When the API still answers 401, it refreshes once and
/// retries; if that fails too, the session is over and the user is sent to the sign-in page.
/// </summary>
public sealed class BearerTokenHandler(AuthSession session, NavigationManager navigation) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? token = await session.AccessTokenAsync();
        HttpResponseMessage response = await SendWithAsync(request, token, cancellationToken);
        if (response.StatusCode != HttpStatusCode.Unauthorized || token is null)
        {
            return response;
        }

        if (await session.RefreshAsync(token) && await CloneAsync(request) is { } retry)
        {
            response.Dispose();
            return await SendWithAsync(retry, await session.AccessTokenAsync(), cancellationToken);
        }

        navigation.NavigateTo(Routes.Login(navigation.ToBaseRelativePath(navigation.Uri)));
        return response;
    }

    private Task<HttpResponseMessage> SendWithAsync(HttpRequestMessage request, string? token, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = token is null ? null : new AuthenticationHeaderValue("Bearer", token);
        return base.SendAsync(request, cancellationToken);
    }

    /// <summary>A copy of a request that has already been sent; requests with a streamed body cannot be repeated.</summary>
    private static async Task<HttpRequestMessage?> CloneAsync(HttpRequestMessage request)
    {
        if (request.Content is StreamContent or MultipartContent)
        {
            return null;
        }

        HttpRequestMessage clone = new(request.Method, request.RequestUri);
        if (request.Content is not null)
        {
            byte[] body = await request.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(body);
            foreach (KeyValuePair<string, IEnumerable<string>> header in request.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return clone;
    }
}
