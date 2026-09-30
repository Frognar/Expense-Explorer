using ExpenseExplorer.Contracts.Common;

namespace ExpenseExplorer.Contracts.ReceiptItems;

/// <summary>A receipt line together with the receipt it belongs to.</summary>
public sealed record ReceiptItemSummaryResponse(
    Guid Id,
    Guid ReceiptId,
    string Store,
    DateOnly PurchaseDate,
    string Item,
    string Category,
    decimal Quantity,
    decimal UnitPrice,
    decimal Amount,
    decimal Discount,
    decimal Total,
    string? Description);

/// <summary>One page of lines plus the total cost of every line matching the filter.</summary>
public sealed record ReceiptItemListResponse(
    PageResponse<ReceiptItemSummaryResponse> Items,
    decimal TotalCost);
