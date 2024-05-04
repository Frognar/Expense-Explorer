namespace ExpenseExplorer.API.Contract;

public record UpdatePurchaseRequest(
  string? Item,
  string? Category,
  decimal? Quantity,
  decimal? UnitPrice,
  decimal? TotalDiscount,
  string? Description);
