namespace ExpenseExplorer.API.Contract;

public record PurchaseResponse(
  string Id,
  string Item,
  string Category,
  decimal Quantity,
  decimal UnitPrice,
  decimal? TotalDiscount,
  string? Description);
