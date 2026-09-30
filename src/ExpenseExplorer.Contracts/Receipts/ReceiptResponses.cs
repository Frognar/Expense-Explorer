using ExpenseExplorer.Contracts.Common;

namespace ExpenseExplorer.Contracts.Receipts;

public sealed record ReceiptResponse(
    Guid Id,
    string Store,
    DateOnly PurchaseDate,
    decimal Total,
    IReadOnlyList<ReceiptItemResponse> Items);

public sealed record ReceiptItemResponse(
    Guid Id,
    string Item,
    string Category,
    decimal Quantity,
    decimal UnitPrice,
    decimal Discount,
    decimal Gross,
    decimal Total,
    string? Description);

public sealed record ReceiptSummaryResponse(
    Guid Id,
    string Store,
    DateOnly PurchaseDate,
    decimal Total);

/// <summary>One page of receipts plus the total cost of every receipt matching the filter.</summary>
public sealed record ReceiptListResponse(
    PageResponse<ReceiptSummaryResponse> Receipts,
    decimal TotalCost);
