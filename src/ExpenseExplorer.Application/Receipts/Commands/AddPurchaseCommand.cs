namespace ExpenseExplorer.Application.Receipts.Commands;

public record AddPurchaseCommand(
  string ReceiptId,
  string ProductName,
  string ProductCategory,
  decimal Quantity,
  decimal UnitPrice,
  decimal? TotalDiscount,
  string? Description);
