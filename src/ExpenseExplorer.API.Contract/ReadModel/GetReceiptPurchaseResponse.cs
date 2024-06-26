namespace ExpenseExplorer.API.Contract.ReadModel;

public record GetReceiptPurchaseResponse(
  string Id,
  string Item,
  string Category,
  decimal Quantity,
  decimal UnitPrice,
  decimal TotalDiscount,
  string Description);
