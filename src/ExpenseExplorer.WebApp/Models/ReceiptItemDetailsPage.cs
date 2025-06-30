namespace ExpenseExplorer.WebApp.Models;

internal sealed record ReceiptItemDetailsPage(
    IEnumerable<ReceiptItemDetails> ReceiptItems,
    int TotalCount,
    decimal TotalCost);