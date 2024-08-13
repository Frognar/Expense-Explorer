using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Receipts.Facts;

public sealed record PurchaseAdded(
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
  public static PurchaseAdded Create(Id receiptId, Purchase purchase)
  {
    ArgumentNullException.ThrowIfNull(receiptId);
    ArgumentNullException.ThrowIfNull(purchase);
    return new PurchaseAdded(
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
