using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Receipts.Facts;

public sealed record StoreCorrected(string ReceiptId, string Store) : Fact
{
  public static StoreCorrected Create(Id receiptId, Store store)
  {
    ArgumentNullException.ThrowIfNull(receiptId);
    ArgumentNullException.ThrowIfNull(store);
    return new StoreCorrected(receiptId.Value, store.Name);
  }
}
