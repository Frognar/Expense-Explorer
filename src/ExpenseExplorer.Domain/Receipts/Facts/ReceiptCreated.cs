namespace ExpenseExplorer.Domain.Receipts.Facts;

using ExpenseExplorer.Domain.ValueObjects;

public class ReceiptCreated(string id, string store, DateOnly purchaseDate, DateOnly createdDate)
  : Fact
{
  public string Id { get; } = id;

  public string Store { get; } = store;

  public DateOnly PurchaseDate { get; } = purchaseDate;

  public DateOnly CreatedDate { get; } = createdDate;

  public static ReceiptCreated Create(Id id, Store store, PurchaseDate purchaseDate, DateOnly createdDate)
  {
    ArgumentNullException.ThrowIfNull(id);
    ArgumentNullException.ThrowIfNull(store);
    ArgumentNullException.ThrowIfNull(purchaseDate);
    ArgumentNullException.ThrowIfNull(createdDate);
    return new ReceiptCreated(id.Value, store.Name, purchaseDate.Date, createdDate);
  }
}
