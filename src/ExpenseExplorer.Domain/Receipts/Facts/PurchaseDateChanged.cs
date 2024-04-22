namespace ExpenseExplorer.Domain.Receipts.Facts;

using ExpenseExplorer.Domain.ValueObjects;

public class PurchaseDateChanged : Fact
{
  public PurchaseDateChanged(Id receiptId, PurchaseDate purchaseDate)
  {
    ReceiptId = receiptId;
    PurchaseDate = purchaseDate;
  }

  public Id ReceiptId { get; }

  public PurchaseDate PurchaseDate { get; }
}
