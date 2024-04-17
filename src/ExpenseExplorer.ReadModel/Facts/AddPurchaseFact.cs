namespace ExpenseExplorer.ReadModel.Facts;

public record AddPurchaseFact(
  string ReceiptId,
  string PurchaseId,
  string Item,
  string Category,
  decimal Quantity,
  decimal UnitPrice,
  decimal TotalDiscount,
  string Description);
