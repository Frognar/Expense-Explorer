namespace ExpenseExplorer.ReadModel.Models;

public sealed record ReceiptHeaders(string Id, string Store, DateOnly PurchaseDate, decimal Total);
