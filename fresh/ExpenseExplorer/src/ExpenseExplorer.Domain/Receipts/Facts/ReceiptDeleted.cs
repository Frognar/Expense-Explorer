using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Receipts.Facts;

public sealed record ReceiptDeleted(string ReceiptId)
  : Fact
{
  public static ReceiptDeleted Create(
    ReceiptIdType receiptId)
    => new(receiptId.Value);
}
