namespace ExpenseExplorer.WebApp.Models;

internal sealed record ReceiptDetailsResponse(
    IEnumerable<ReceiptDetails> Receipts,
    int TotalCount,
    decimal TotalCost);