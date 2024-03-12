namespace ExpenseExplorer.API.Contract;

public record AddPurchaseRequest(
  string Item,
  string Category,
  decimal Quantity,
  decimal UnitPrice,
  decimal? TotalDiscount,
  string? Description);
