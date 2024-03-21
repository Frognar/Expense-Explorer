namespace ExpenseExplorer.API.Contract;

public record PurchaseResponse(
  int Index,
  string Item,
  string Category,
  decimal Quantity,
  decimal UnitPrice,
  decimal? TotalDiscount,
  string? Description);
