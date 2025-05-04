namespace ExpenseExplorer.WebApp.Models;

internal sealed record ReceiptDetailsPage(
    IEnumerable<ReceiptDetails> Receipts,
    int TotalCount,
    decimal TotalCost);