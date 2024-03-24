namespace ExpenseExplorer.Domain.Receipts.Events;

using ExpenseExplorer.Domain.ValueObjects;

public class ReceiptCreated : Fact
{
  public ReceiptCreated(Id id, Store store, PurchaseDate purchaseDate, DateOnly createdDate)
  {
    Id = id;
    Store = store;
    PurchaseDate = purchaseDate;
    CreatedDate = createdDate;
  }

  public Id Id { get; }

  public Store Store { get; }

  public PurchaseDate PurchaseDate { get; }

  public DateOnly CreatedDate { get; }
}
