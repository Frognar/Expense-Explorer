using ExpenseExplorer.Contracts.Logs;
using ExpenseExplorer.Contracts.ReceiptItems;
using ExpenseExplorer.Contracts.Receipts;

namespace ExpenseExplorer.Web.Api;

/// <summary>
/// List filters as query strings. The pages keep their filters in the address using the same
/// names as the API, so a filtered view can be bookmarked, shared or reached with the back button.
/// </summary>
public static class ListQueries
{
    public static string ToQuery(ReceiptListRequest request) =>
        new QueryString()
            .Add("stores", request.Stores)
            .Add("from", request.From)
            .Add("to", request.To)
            .Add("totalMin", request.TotalMin)
            .Add("totalMax", request.TotalMax)
            .Add("sortBy", request.SortBy)
            .Add("direction", request.Direction)
            .Add("page", request.Page)
            .Add("pageSize", request.PageSize)
            .ToString();

    public static string ToQuery(ReceiptItemListRequest request) =>
        new QueryString()
            .Add("stores", request.Stores)
            .Add("items", request.Items)
            .Add("categories", request.Categories)
            .Add("from", request.From)
            .Add("to", request.To)
            .Add("quantityMin", request.QuantityMin)
            .Add("quantityMax", request.QuantityMax)
            .Add("unitPriceMin", request.UnitPriceMin)
            .Add("unitPriceMax", request.UnitPriceMax)
            .Add("amountMin", request.AmountMin)
            .Add("amountMax", request.AmountMax)
            .Add("discountMin", request.DiscountMin)
            .Add("discountMax", request.DiscountMax)
            .Add("totalMin", request.TotalMin)
            .Add("totalMax", request.TotalMax)
            .Add("description", request.Description)
            .Add("sortBy", request.SortBy)
            .Add("direction", request.Direction)
            .Add("page", request.Page)
            .Add("pageSize", request.PageSize)
            .ToString();

    public static string ToQuery(LogListRequest request) =>
        new QueryString()
            .Add("day", request.Day)
            .Add("level", request.Level)
            .Add("search", request.Search)
            .Add("page", request.Page)
            .Add("pageSize", request.PageSize)
            .ToString();

    public static ReceiptItemListRequest ReceiptItemList(QueryReader query) =>
        new()
        {
            Stores = query.Texts("stores"),
            Items = query.Texts("items"),
            Categories = query.Texts("categories"),
            From = query.Date("from"),
            To = query.Date("to"),
            QuantityMin = query.Number("quantityMin"),
            QuantityMax = query.Number("quantityMax"),
            UnitPriceMin = query.Number("unitPriceMin"),
            UnitPriceMax = query.Number("unitPriceMax"),
            AmountMin = query.Number("amountMin"),
            AmountMax = query.Number("amountMax"),
            DiscountMin = query.Number("discountMin"),
            DiscountMax = query.Number("discountMax"),
            TotalMin = query.Number("totalMin"),
            TotalMax = query.Number("totalMax"),
            Description = query.Text("description"),
            SortBy = query.Enum<ReceiptItemSortField>("sortBy"),
            Direction = query.Enum<Contracts.Common.SortDirection>("direction"),
            Page = query.WholeNumber("page"),
            PageSize = query.WholeNumber("pageSize"),
        };

    public static ReceiptListRequest ReceiptList(QueryReader query) =>
        new()
        {
            Stores = query.Texts("stores"),
            From = query.Date("from"),
            To = query.Date("to"),
            TotalMin = query.Number("totalMin"),
            TotalMax = query.Number("totalMax"),
            SortBy = query.Enum<ReceiptSortField>("sortBy"),
            Direction = query.Enum<Contracts.Common.SortDirection>("direction"),
            Page = query.WholeNumber("page"),
            PageSize = query.WholeNumber("pageSize"),
        };

    public static LogListRequest LogList(QueryReader query) =>
        new()
        {
            Day = query.Date("day"),
            Level = query.Enum<LogSeverity>("level"),
            Search = query.Text("search"),
            Page = query.WholeNumber("page"),
            PageSize = query.WholeNumber("pageSize"),
        };
}
