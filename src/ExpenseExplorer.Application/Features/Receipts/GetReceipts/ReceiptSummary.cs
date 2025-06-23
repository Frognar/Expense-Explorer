namespace ExpenseExplorer.Application.Features.Receipts.GetReceipts;

public sealed record ReceiptSummary(
    Guid Id,
    string Store,
    DateOnly PurchaseDate,
    decimal Total);