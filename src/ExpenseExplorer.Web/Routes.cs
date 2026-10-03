using ExpenseExplorer.Contracts.Logs;
using ExpenseExplorer.Contracts.ReceiptItems;
using ExpenseExplorer.Contracts.Receipts;
using ExpenseExplorer.Web.Api;

namespace ExpenseExplorer.Web;

/// <summary>Addresses of the app's pages, in one place.</summary>
public static class Routes
{
    public const string Reports = "reports";

    // "./" rather than "": links and buttons treat an empty address as no link at all.
    public static string Receipts(ReceiptListRequest? filter = null) =>
        "./" + (filter is null ? "" : ListQueries.ToQuery(filter));

    public static string ReceiptItems(ReceiptItemListRequest? filter = null) =>
        "receipt-items" + (filter is null ? "" : ListQueries.ToQuery(filter));

    public static string Receipt(Guid id) => $"receipts/{id}";

    public static string Logs(LogListRequest? filter = null) =>
        "logs" + (filter is null ? "" : ListQueries.ToQuery(filter));

    public static string Budget(Guid? periodId = null) =>
        "budget" + new QueryString().Add("period", periodId?.ToString());

    public static string Dictionaries(Suggestions kind = Suggestions.Items, string? search = null) =>
        "dictionaries" + new QueryString().Add("kind", ExpenseApi.PathOf(kind)).Add("search", search);

    public static string Login(string? returnPath = null) =>
        string.IsNullOrEmpty(returnPath) || returnPath.StartsWith("login", StringComparison.Ordinal)
            ? "login"
            : $"login?returnPath={Uri.EscapeDataString(returnPath)}";
}
