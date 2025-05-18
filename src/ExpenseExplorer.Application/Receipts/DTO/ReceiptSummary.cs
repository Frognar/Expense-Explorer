namespace ExpenseExplorer.Application.Receipts.DTO;

public sealed record ReceiptSummary(string Store, DateOnly PurchaseDate, decimal Total);