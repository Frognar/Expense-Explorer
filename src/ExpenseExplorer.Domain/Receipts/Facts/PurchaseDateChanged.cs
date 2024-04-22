namespace ExpenseExplorer.Domain.Receipts.Facts;

using ExpenseExplorer.Domain.ValueObjects;

public class PurchaseDateChanged : Fact
{
  public PurchaseDateChanged(Id receiptId, PurchaseDate purchaseDate, DateOnly requestedAt)
  {
    ReceiptId = receiptId;
    PurchaseDate = purchaseDate;
    RequestedAt = requestedAt;
  }

  public Id ReceiptId { get; }

  public PurchaseDate PurchaseDate { get; }

  public DateOnly RequestedAt { get; }
}
