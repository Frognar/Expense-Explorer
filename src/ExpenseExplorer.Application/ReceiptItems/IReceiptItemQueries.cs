using ExpenseExplorer.Contracts.Common;
using ExpenseExplorer.Contracts.ReceiptItems;

namespace ExpenseExplorer.Application.ReceiptItems;

public interface IReceiptItemQueries
{
    Task<ReceiptItemListResponse> ListAsync(ReceiptItemListQuery query, CancellationToken cancellationToken);
}

/// <summary>An inclusive range; either end may be open.</summary>
public sealed record Range<T>(T? Min, T? Max)
    where T : struct;

/// <summary>Validated form of <see cref="ReceiptItemListRequest"/>: ranges are ordered, paging is within limits.</summary>
public sealed record ReceiptItemListQuery(
    IReadOnlyList<string> Stores,
    IReadOnlyList<string> Items,
    IReadOnlyList<string> Categories,
    Range<DateOnly> PurchaseDate,
    ReceiptItemAmountFilters Amounts,
    string? Description,
    ReceiptItemSortField SortBy,
    SortDirection Direction,
    int Page,
    int PageSize);

public sealed record ReceiptItemAmountFilters(
    Range<decimal> Quantity,
    Range<decimal> UnitPrice,
    Range<decimal> Amount,
    Range<decimal> Discount,
    Range<decimal> Total);
