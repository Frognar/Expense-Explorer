using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Receipts.Facts;

public sealed record ReceiptCreated(
  string ReceiptId,
  string Store,
  DateOnly PurchaseDate,
  DateOnly CreatedDate)
  : Fact
{
  public static ReceiptCreated Create(
    ReceiptIdType receiptId,
    StoreType store,
    NonFutureDateType purchaseDate)
    => new(receiptId.Value, store.Value, purchaseDate.Value, purchaseDate.CreationDate);
}
