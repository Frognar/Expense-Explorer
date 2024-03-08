namespace ExpenseExplorer.Domain.Receipts;

using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;

public class Receipt
{
  private readonly List<Fact> changes;

  private Receipt(Id id, Store store, PurchaseDate purchaseDate, List<Fact> changes)
  {
    Id = id;
    Store = store;
    PurchaseDate = purchaseDate;
    this.changes = changes;
  }

  public Id Id { get; }

  public Store Store { get; }

  public PurchaseDate PurchaseDate { get; }

  public IEnumerable<Fact> UnsavedChanges => changes;

  public static Receipt New(Store store, PurchaseDate purchaseDate)
  {
    Id id = Id.Unique();
    return new Receipt(id, store, purchaseDate, [new ReceiptCreated(id, store, purchaseDate)]);
  }
}
