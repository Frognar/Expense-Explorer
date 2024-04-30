namespace ExpenseExplorer.Domain.Receipts.Facts;

using ExpenseExplorer.Domain.ValueObjects;

public sealed record ReceiptCreated(string Id, string Store, DateOnly PurchaseDate, DateOnly CreatedDate) : Fact
{
  public static ReceiptCreated Create(Id id, Store store, PurchaseDate purchaseDate, DateOnly createdDate)
  {
    ArgumentNullException.ThrowIfNull(id);
    ArgumentNullException.ThrowIfNull(store);
    ArgumentNullException.ThrowIfNull(purchaseDate);
    ArgumentNullException.ThrowIfNull(createdDate);
    return new ReceiptCreated(id.Value, store.Name, purchaseDate.Date, createdDate);
  }
}
