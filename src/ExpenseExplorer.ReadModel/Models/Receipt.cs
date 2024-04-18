namespace ExpenseExplorer.ReadModel.Models;

public record Receipt(string Id, string Store, DateOnly PurchaseDate, decimal Total);
