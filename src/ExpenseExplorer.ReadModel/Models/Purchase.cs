namespace ExpenseExplorer.ReadModel.Models;

public record Purchase(
  string Id,
  string Item,
  string Category,
  decimal Quantity,
  decimal UnitPrice,
  decimal TotalDiscount,
  string Description);
