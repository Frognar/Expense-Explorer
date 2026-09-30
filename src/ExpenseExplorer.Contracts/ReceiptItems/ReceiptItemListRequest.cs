using System.Diagnostics.CodeAnalysis;
using ExpenseExplorer.Contracts.Common;

namespace ExpenseExplorer.Contracts.ReceiptItems;

public enum ReceiptItemSortField
{
    PurchaseDate,
    Store,
    Item,
    Category,
    Quantity,
    UnitPrice,
    Amount,
    Discount,
    Total,
    Description,
}

/// <summary>
/// Query string of <c>GET /api/v1/receipt-items</c>. Every filter is optional; list filters
/// match any of the given values, ranges include both ends.
/// </summary>
[SuppressMessage("Performance", "CA1819", Justification = "Minimal API binds repeated query parameters to arrays.")]
public sealed record ReceiptItemListRequest
{
    public const int DefaultPageSize = 20;

    public const int MaxPageSize = 100;

    public string[]? Stores { get; init; }

    public string[]? Items { get; init; }

    public string[]? Categories { get; init; }

    public DateOnly? From { get; init; }

    public DateOnly? To { get; init; }

    public decimal? QuantityMin { get; init; }

    public decimal? QuantityMax { get; init; }

    public decimal? UnitPriceMin { get; init; }

    public decimal? UnitPriceMax { get; init; }

    public decimal? AmountMin { get; init; }

    public decimal? AmountMax { get; init; }

    public decimal? DiscountMin { get; init; }

    public decimal? DiscountMax { get; init; }

    public decimal? TotalMin { get; init; }

    public decimal? TotalMax { get; init; }

    /// <summary>Part of the description, case does not matter.</summary>
    public string? Description { get; init; }

    public ReceiptItemSortField? SortBy { get; init; }

    public SortDirection? Direction { get; init; }

    public int? Page { get; init; }

    public int? PageSize { get; init; }
}
