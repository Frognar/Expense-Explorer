using ExpenseExplorer.Api.Dictionaries;
using ExpenseExplorer.Api.Receipts;

namespace ExpenseExplorer.Api;

internal static class Routing
{
    public const string ApiPrefix = "/api/v1";

    public static WebApplication MapApi(this WebApplication app)
    {
        RouteGroupBuilder api = app.MapGroup(ApiPrefix);
        api.MapReceipts();
        api.MapDictionaries();

        // Unknown API routes answer with ProblemDetails instead of falling through to the frontend.
        api.MapFallback(() => Results.Problem(statusCode: StatusCodes.Status404NotFound));

        return app;
    }

    /// <summary>Client-side routes of the frontend are served by its index.html.</summary>
    public static WebApplication MapFrontendFallback(this WebApplication app)
    {
        app.MapFallbackToFile("index.html");
        return app;
    }
}
