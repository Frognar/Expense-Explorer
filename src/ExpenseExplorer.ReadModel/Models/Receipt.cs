namespace ExpenseExplorer.ReadModel.Models;

public sealed record Receipt(
  string Id,
  string Store,
  DateOnly PurchaseDate,
  decimal Total,
  IEnumerable<Purchase> Purchases);
