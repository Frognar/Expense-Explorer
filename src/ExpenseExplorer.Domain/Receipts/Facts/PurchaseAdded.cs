namespace ExpenseExplorer.Domain.Receipts.Facts;

using ExpenseExplorer.Domain.ValueObjects;

public class PurchaseAdded : Fact
{
  public PurchaseAdded(Id receiptId, Purchase purchase)
  {
    ReceiptId = receiptId;
    Purchase = purchase;
  }

  public Id ReceiptId { get; }

  public Purchase Purchase { get; }
}
