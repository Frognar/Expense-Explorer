using System.Diagnostics.CodeAnalysis;
using ExpenseExplorer.Contracts.Common;

namespace ExpenseExplorer.Contracts.Receipts;

public enum ReceiptSortField
{
    PurchaseDate,
    Store,
    Total,
}

/// <summary>Query string of <c>GET /api/v1/receipts</c>. Every filter is optional.</summary>
public sealed record ReceiptListRequest
{
    public const int DefaultPageSize = 20;

    public const int MaxPageSize = 100;

    [SuppressMessage("Performance", "CA1819", Justification = "Minimal API binds repeated query parameters to arrays.")]
    public string[]? Stores { get; init; }

    public DateOnly? From { get; init; }

    public DateOnly? To { get; init; }

    public decimal? TotalMin { get; init; }

    public decimal? TotalMax { get; init; }

    public ReceiptSortField? SortBy { get; init; }

    public SortDirection? Direction { get; init; }

    public int? Page { get; init; }

    public int? PageSize { get; init; }
}
