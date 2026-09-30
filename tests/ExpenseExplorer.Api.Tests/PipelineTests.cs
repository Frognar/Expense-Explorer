using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseExplorer.Api.Tests;

public class PipelineTests(ApiFixture api)
{
    private readonly HttpClient _client = api.Client;

    [Fact]
    public async Task Health_endpoint_reports_healthy()
    {
        HttpResponseMessage response = await _client.GetAsync(new Uri("/health", UriKind.Relative), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Unknown_api_route_returns_problem_details()
    {
        HttpResponseMessage response = await _client.GetAsync(new Uri("/api/v1/does-not-exist", UriKind.Relative), TestContext.Current.CancellationToken);
        ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(404, problem?.Status);
    }

    [Fact]
    public async Task Malformed_json_returns_bad_request_problem()
    {
        using StringContent body = new("{ not json", System.Text.Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _client.PostAsync(new Uri("/api/v1/receipts", UriKind.Relative), body, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }
}
