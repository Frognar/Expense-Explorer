namespace ExpenseExplorer.Domain.Receipts.Facts;

using ExpenseExplorer.Domain.ValueObjects;

public class StoreCorrected : Fact
{
  public StoreCorrected(Id receiptId, Store store)
  {
    ReceiptId = receiptId;
    Store = store;
  }

  public Id ReceiptId { get; }

  public Store Store { get; }
}
