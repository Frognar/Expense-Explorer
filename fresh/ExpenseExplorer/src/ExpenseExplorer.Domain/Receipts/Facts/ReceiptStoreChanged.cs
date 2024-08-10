using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Receipts.Facts;

public sealed record ReceiptStoreChanged(
  string ReceiptId,
  string Store)
  : Fact
{
  public static ReceiptStoreChanged Create(
    ReceiptIdType receiptId,
    StoreType store)
    => new(receiptId.Value, store.Value);
}
