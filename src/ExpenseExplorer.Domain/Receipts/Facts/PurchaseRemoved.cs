namespace ExpenseExplorer.Domain.Receipts.Facts;

using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

public record PurchaseRemoved(string ReceiptId, string PurchaseId) : Fact
{
  public static PurchaseRemoved Create(Id receiptId, Id purchaseId)
  {
    ArgumentNullException.ThrowIfNull(receiptId);
    ArgumentNullException.ThrowIfNull(purchaseId);
    return new PurchaseRemoved(receiptId.Value, purchaseId.Value);
  }
}
