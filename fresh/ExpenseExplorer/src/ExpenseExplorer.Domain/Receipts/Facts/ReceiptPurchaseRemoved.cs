using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Receipts.Facts;

public sealed record ReceiptPurchaseRemoved(
  string ReceiptId,
  string PurchaseId)
  : Fact
{
  public static ReceiptPurchaseRemoved Create(
    ReceiptIdType receiptId,
    PurchaseIdType purchaseId)
    => new(receiptId.Value, purchaseId.Value);
}
