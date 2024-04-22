namespace ExpenseExplorer.Domain.Receipts.Facts;

using ExpenseExplorer.Domain.ValueObjects;

public class PurchaseAdded(
  string receiptId,
  string purchaseId,
  string item,
  string category,
  decimal quantity,
  decimal unitPrice,
  decimal totalDiscount,
  string description)
  : Fact
{
  public string ReceiptId { get; } = receiptId;

  public string PurchaseId { get; } = purchaseId;

  public string Item { get; } = item;

  public string Category { get; } = category;

  public decimal Quantity { get; } = quantity;

  public decimal UnitPrice { get; } = unitPrice;

  public decimal TotalDiscount { get; } = totalDiscount;

  public string Description { get; } = description;

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
