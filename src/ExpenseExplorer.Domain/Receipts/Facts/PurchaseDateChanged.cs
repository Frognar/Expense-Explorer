namespace ExpenseExplorer.Domain.Receipts.Facts;

using ExpenseExplorer.Domain.ValueObjects;

public class PurchaseDateChanged(string receiptId, DateOnly purchaseDate, DateOnly requestedDate) : Fact
{
  public string ReceiptId { get; } = receiptId;

  public DateOnly PurchaseDate { get; } = purchaseDate;

  public DateOnly RequestedDate { get; } = requestedDate;

  public static PurchaseDateChanged Create(Id receiptId, PurchaseDate purchaseDate, DateOnly requestedDate)
  {
    ArgumentNullException.ThrowIfNull(receiptId);
    ArgumentNullException.ThrowIfNull(purchaseDate);
    return new PurchaseDateChanged(receiptId.Value, purchaseDate.Date, requestedDate);
  }
}
