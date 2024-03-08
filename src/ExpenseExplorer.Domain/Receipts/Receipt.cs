namespace ExpenseExplorer.Domain.Receipts;

using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;

public class Receipt
{
  private readonly IEnumerable<Fact> changes;

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
    Fact receiptCreated = new ReceiptCreated(id, store, purchaseDate);
    return new Receipt(id, store, purchaseDate, [receiptCreated]);
  }

  public Receipt CorrectStore(Store store)
  {
    Fact storeCorrected = new StoreCorrected(Id, store);
    List<Fact> allChanges = changes.Append(storeCorrected).ToList();
    return new Receipt(Id, store, PurchaseDate, allChanges);
  }
}
