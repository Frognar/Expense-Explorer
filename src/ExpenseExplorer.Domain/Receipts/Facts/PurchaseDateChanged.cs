using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Receipts.Facts;

public sealed record PurchaseDateChanged(string ReceiptId, DateOnly PurchaseDate, DateOnly RequestedDate) : Fact
{
  public static PurchaseDateChanged Create(Id receiptId, NonFutureDate purchaseDate, DateOnly requestedDate)
  {
    ArgumentNullException.ThrowIfNull(receiptId);
    ArgumentNullException.ThrowIfNull(purchaseDate);
    return new PurchaseDateChanged(receiptId.Value, purchaseDate.Date, requestedDate);
  }
}
