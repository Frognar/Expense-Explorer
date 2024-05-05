namespace ExpenseExplorer.ReadModel.Models;

public sealed record Purchase(
  string Id,
  string Item,
  string Category,
  decimal Quantity,
  decimal UnitPrice,
  decimal TotalDiscount,
  string Description);
