using ExpenseExplorer.Api.Http;
using ExpenseExplorer.Application.ReceiptItems;
using ExpenseExplorer.Contracts.Common;
using ExpenseExplorer.Contracts.ReceiptItems;
using ExpenseExplorer.Domain.Common;

namespace ExpenseExplorer.Api.ReceiptItems;

internal static class ReceiptItemRequestParser
{
    public static Result<ReceiptItemListQuery> ParseList(ReceiptItemListRequest request) =>
        ResultCombine.Combine(
            Range(request.From, request.To, "from"),
            Amounts(request),
            Input.InRange(request.Page, 1, 1, int.MaxValue).ForTarget("page"),
            Input.InRange(request.PageSize, ReceiptItemListRequest.DefaultPageSize, 1, ReceiptItemListRequest.MaxPageSize).ForTarget("pageSize"),
            (dates, amounts, page, pageSize) => new ReceiptItemListQuery(
                Input.NonBlankDistinct(request.Stores),
                Input.NonBlankDistinct(request.Items),
                Input.NonBlankDistinct(request.Categories),
                dates,
                amounts,
                Input.NonBlank(request.Description),
                request.SortBy ?? ReceiptItemSortField.PurchaseDate,
                request.Direction ?? SortDirection.Descending,
                page,
                pageSize));

    private static Result<ReceiptItemAmountFilters> Amounts(ReceiptItemListRequest request) =>
        ResultCombine.Combine(
            Range(request.QuantityMin, request.QuantityMax, "quantityMin"),
            Range(request.UnitPriceMin, request.UnitPriceMax, "unitPriceMin"),
            Range(request.AmountMin, request.AmountMax, "amountMin"),
            Range(request.DiscountMin, request.DiscountMax, "discountMin"),
            Range(request.TotalMin, request.TotalMax, "totalMin"),
            (quantity, unitPrice, amount, discount, total) => new ReceiptItemAmountFilters(quantity, unitPrice, amount, discount, total));

    private static Result<Range<T>> Range<T>(T? min, T? max, string target)
        where T : struct, IComparable<T> =>
        Input.OrderedRange(min, max).ForTarget(target).Map(range => new Range<T>(range.From, range.To));
}
