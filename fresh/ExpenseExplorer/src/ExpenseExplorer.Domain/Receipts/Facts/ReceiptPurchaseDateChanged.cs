using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Receipts.Facts;

public sealed record ReceiptPurchaseDateChanged(
  string ReceiptId,
  DateOnly PurchaseDate,
  DateOnly CreatedDate)
  : Fact
{
  public static ReceiptPurchaseDateChanged Create(
    ReceiptIdType receiptId,
    NonFutureDateType purchaseDate)
    => new(receiptId.Value, purchaseDate.Value, purchaseDate.CreationDate);
}
