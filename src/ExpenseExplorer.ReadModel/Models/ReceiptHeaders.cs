namespace ExpenseExplorer.ReadModel.Models;

public record ReceiptHeaders(string Id, string Store, DateOnly PurchaseDate, decimal Total);
