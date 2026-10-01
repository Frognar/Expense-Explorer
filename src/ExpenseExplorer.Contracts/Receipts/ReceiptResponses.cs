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
    decimal Amount,
    decimal Discount,
    decimal Total,
    decimal UnitPrice,
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

/// <summary>
/// A receipt read from a photo, with what to check against the paper: <paramref name="PrintedTotal"/>
/// should equal the receipt total, and the lines in <paramref name="SkippedLines"/> could not be read.
/// When <paramref name="PurchaseDateFound"/> is false, the purchase date is today.
/// </summary>
public sealed record PhotoImportResponse(
    ReceiptResponse Receipt,
    decimal? PrintedTotal,
    bool PurchaseDateFound,
    IReadOnlyList<string> SkippedLines);
