namespace ExpenseExplorer.Application.Features.Receipts.GetReceipts;

public sealed record GetReceiptSummariesResponse(
    PageOf<ReceiptSummary> Receipts,
    decimal Total);