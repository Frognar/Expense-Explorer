using ExpenseExplorer.Api.Auth;
using ExpenseExplorer.Api.Dictionaries;
using ExpenseExplorer.Api.ReceiptItems;
using ExpenseExplorer.Api.Receipts;
using ExpenseExplorer.Api.Reports;

namespace ExpenseExplorer.Api;

internal static class Routing
{
    public const string ApiPrefix = "/api/v1";

    public static WebApplication MapApi(this WebApplication app)
    {
        RouteGroupBuilder api = app.MapGroup(ApiPrefix);
        api.MapAuth();

        // Everything else needs a signed-in user; endpoints that change data also need the editor role.
        RouteGroupBuilder data = api.MapGroup("").RequireAuthorization(Policies.CanRead);
        data.MapReceipts();
        data.MapReceiptItems();
        data.MapReports();
        data.MapDictionaries();

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
