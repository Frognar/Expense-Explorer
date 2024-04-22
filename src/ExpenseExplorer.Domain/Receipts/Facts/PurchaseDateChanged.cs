namespace ExpenseExplorer.Domain.Receipts.Facts;

using ExpenseExplorer.Domain.ValueObjects;

public class PurchaseDateChanged : Fact
{
  public PurchaseDateChanged(Id receiptId, PurchaseDate purchaseDate, DateOnly requestedDate)
  {
    ReceiptId = receiptId;
    PurchaseDate = purchaseDate;
    RequestedDate = requestedDate;
  }

  public Id ReceiptId { get; }

  public PurchaseDate PurchaseDate { get; }

  public DateOnly RequestedDate { get; }
}
