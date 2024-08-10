using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Receipts.Facts;

public sealed record ReceiptPurchaseAdded(
  string ReceiptId,
  string PurchaseId)
  : Fact
{
  public static ReceiptPurchaseAdded Create(
    ReceiptIdType receiptId,
    PurchaseIdType purchaseId)
    => new(receiptId.Value, purchaseId.Value);
}
