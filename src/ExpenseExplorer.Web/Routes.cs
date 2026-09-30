using ExpenseExplorer.Contracts.ReceiptItems;
using ExpenseExplorer.Contracts.Receipts;
using ExpenseExplorer.Web.Api;

namespace ExpenseExplorer.Web;

/// <summary>Addresses of the app's pages, in one place.</summary>
public static class Routes
{
    public const string Reports = "reports";

    public static string Receipts(ReceiptListRequest? filter = null) =>
        filter is null ? "" : ListQueries.ToQuery(filter);

    public static string ReceiptItems(ReceiptItemListRequest? filter = null) =>
        "receipt-items" + (filter is null ? "" : ListQueries.ToQuery(filter));

    public static string Receipt(Guid id) => $"receipts/{id}";

    public static string Login(string? returnPath = null) =>
        string.IsNullOrEmpty(returnPath) || returnPath.StartsWith("login", StringComparison.Ordinal)
            ? "login"
            : $"login?returnPath={Uri.EscapeDataString(returnPath)}";
}
