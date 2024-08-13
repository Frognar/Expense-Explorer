using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Receipts.Facts;

public record ReceiptDeleted(string ReceiptId) : Fact
{
  public static ReceiptDeleted Create(Id receiptId)
  {
    ArgumentNullException.ThrowIfNull(receiptId);
    return new ReceiptDeleted(receiptId.Value);
  }
}
