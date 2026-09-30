using System.Net.Http.Json;
using System.Text.Json;

namespace ExpenseExplorer.Api.Tests;

internal static class Http
{
    private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

    public static Task<HttpResponseMessage> Get(this HttpClient client, string url) =>
        client.GetAsync(new Uri(url, UriKind.Relative), Cancellation);

    public static Task<HttpResponseMessage> Post(this HttpClient client, string url, object body) =>
        client.PostAsJsonAsync(new Uri(url, UriKind.Relative), body, Cancellation);

    public static Task<HttpResponseMessage> Put(this HttpClient client, string url, object body) =>
        client.PutAsJsonAsync(new Uri(url, UriKind.Relative), body, Cancellation);

    public static Task<HttpResponseMessage> Patch(this HttpClient client, string url, object body) =>
        client.PatchAsJsonAsync(new Uri(url, UriKind.Relative), body, Cancellation);

    public static Task<HttpResponseMessage> Delete(this HttpClient client, string url) =>
        client.DeleteAsync(new Uri(url, UriKind.Relative), Cancellation);

    public static async Task<T> Read<T>(this HttpResponseMessage response) =>
        await response.Content.ReadFromJsonAsync<T>(Cancellation)
        ?? throw new InvalidOperationException("Response body is empty.");

    /// <summary>The <c>errorCodes</c> extension of a ProblemDetails response: input name → codes.</summary>
    public static async Task<Dictionary<string, string[]>> ErrorCodes(this HttpResponseMessage response)
    {
        using JsonDocument problem = JsonDocument.Parse(await response.Content.ReadAsStringAsync(Cancellation));
        return problem.RootElement.GetProperty("errorCodes").Deserialize<Dictionary<string, string[]>>()
            ?? [];
    }
}
