namespace ExpenseExplorer.API.Contract;

public record AddPurchaseRequest(
  string ProductName,
  string ProductCategory,
  decimal Quantity,
  decimal UnitPrice,
  decimal? TotalDiscount,
  string? Description);
