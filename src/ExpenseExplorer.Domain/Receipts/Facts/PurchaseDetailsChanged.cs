namespace ExpenseExplorer.Domain.Receipts.Facts;

using ExpenseExplorer.Domain.ValueObjects;

public record PurchaseDetailsChanged(
  string ReceiptId,
  string PurchaseId,
  string Item,
  string Category,
  decimal Quantity,
  decimal UnitPrice,
  decimal TotalDiscount,
  string Description)
  : Fact
{
  public static PurchaseDetailsChanged Create(Id receiptId, Purchase purchase)
  {
    ArgumentNullException.ThrowIfNull(receiptId);
    ArgumentNullException.ThrowIfNull(purchase);
    return new PurchaseDetailsChanged(
      receiptId.Value,
      purchase.Id.Value,
      purchase.Item.Name,
      purchase.Category.Name,
      purchase.Quantity.Value,
      purchase.UnitPrice.Value,
      purchase.TotalDiscount.Value,
      purchase.Description.Value);
  }
}
