namespace ExpenseExplorer.Domain.Receipts.Facts;

using ExpenseExplorer.Domain.ValueObjects;

public class StoreCorrected(string receiptId, string store) : Fact
{
  public string ReceiptId { get; } = receiptId;

  public string Store { get; } = store;

  public static StoreCorrected Create(Id receiptId, Store store)
  {
    ArgumentNullException.ThrowIfNull(receiptId);
    ArgumentNullException.ThrowIfNull(store);
    return new StoreCorrected(receiptId.Value, store.Name);
  }
}
