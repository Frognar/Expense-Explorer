namespace ExpenseExplorer.Domain.Receipts;

using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;

public class Receipt
{
  private readonly List<Fact> changes = [];

  private Receipt(Id id, Store store, PurchaseDate purchaseDate)
  {
    ReceiptCreated fact = new(id, store, purchaseDate);
    Apply(fact);
    changes.Add(fact);
  }

  public Id Id { get; private set; } = null!;

  public Store Store { get; private set; } = null!;

  public PurchaseDate PurchaseDate { get; private set; } = null!;

  public IEnumerable<Fact> UnsavedChanges => changes;

  public static Receipt New(Store store, PurchaseDate purchaseDate)
  {
    return new Receipt(Id.Unique(), store, purchaseDate);
  }

  private void Apply(ReceiptCreated e)
  {
    Id = e.Id;
    Store = e.Store;
    PurchaseDate = e.PurchaseDate;
  }
}
