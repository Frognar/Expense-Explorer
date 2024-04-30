namespace ExpenseExplorer.Domain.Receipts.Facts;

using ExpenseExplorer.Domain.ValueObjects;

public sealed record StoreCorrected(string ReceiptId, string Store) : Fact
{
  public static StoreCorrected Create(Id receiptId, Store store)
  {
    ArgumentNullException.ThrowIfNull(receiptId);
    ArgumentNullException.ThrowIfNull(store);
    return new StoreCorrected(receiptId.Value, store.Name);
  }
}
