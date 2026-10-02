using ExpenseExplorer.Api.Http;
using ExpenseExplorer.Contracts.Logs;
using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Api.Logs;

internal static class LogEndpoints
{
    public static RouteGroupBuilder MapLogs(this RouteGroupBuilder api)
    {
        api.MapGet("/logs", ListAsync).WithTags("Logs");
        return api;
    }

    public static WebApplicationBuilder AddLogFiles(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<LogOptions>(builder.Configuration.GetSection(LogOptions.Section));
        builder.Services.AddSingleton<LogFiles>();
        return builder;
    }

    private static Task<IResult> ListAsync(
        [AsParameters] LogListRequest request,
        LogFiles logs,
        TimeProvider clock,
        CancellationToken cancellationToken) =>
        ParseList(request, clock.Today()).ToHttpAsync(
            async query => Result.Success(await logs.ListAsync(query, cancellationToken)),
            list => Results.Ok(list));

    internal static Result<LogQuery> ParseList(LogListRequest request, DateOnly today) =>
        ResultCombine.Combine(
            Input.InRange(request.Page, 1, 1, int.MaxValue).ForTarget("page"),
            Input.InRange(request.PageSize, LogListRequest.DefaultPageSize, 1, LogListRequest.MaxPageSize).ForTarget("pageSize"),
            (page, pageSize) => new LogQuery(
                request.Day ?? today,
                request.Level ?? LogSeverity.Verbose,
                Input.NonBlank(request.Search),
                page,
                pageSize));
}
