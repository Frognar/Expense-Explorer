namespace ExpenseExplorer.Domain.Receipts;

using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;

public class Receipt
{
  private static readonly Receipt Empty = new();
  private readonly IEnumerable<Fact> changes;

  private Receipt()
    : this(Id.Create("[EMPTY]"), Store.Create("[EMPTY]"), PurchaseDate.MinValue, [], [])
  {
  }

  private Receipt(Id id, Store store, PurchaseDate purchaseDate, ICollection<Purchase> purchases, List<Fact> changes)
  {
    Id = id;
    Store = store;
    PurchaseDate = purchaseDate;
    Purchases = purchases;
    this.changes = changes;
  }

  public Id Id { get; }

  public Store Store { get; }

  public PurchaseDate PurchaseDate { get; }

  public ICollection<Purchase> Purchases { get; }

  public IEnumerable<Fact> UnsavedChanges => changes;

  public static Receipt New(Store store, PurchaseDate purchaseDate)
  {
    Id id = Id.Unique();
    Fact receiptCreated = new ReceiptCreated(id, store, purchaseDate);
    return new Receipt(id, store, purchaseDate, [], [receiptCreated]);
  }

  public static Receipt Recreate(IEnumerable<Fact> events)
  {
    return events.Aggregate(Empty, (receipt, fact) => receipt.ApplyEvent(fact));
  }

  public Receipt ClearChanges()
  {
    return new Receipt(Id, Store, PurchaseDate, Purchases, []);
  }

  public Receipt CorrectStore(Store store)
  {
    Fact storeCorrected = new StoreCorrected(Id, store);
    List<Fact> allChanges = changes.Append(storeCorrected).ToList();
    return new Receipt(Id, store, PurchaseDate, Purchases, allChanges);
  }

  public Receipt AddPurchase(Purchase purchase)
  {
    Fact purchaseAdded = new PurchaseAdded(Id, purchase);
    List<Fact> allChanges = changes.Append(purchaseAdded).ToList();
    List<Purchase> allPurchases = Purchases.Append(purchase).ToList();
    return new Receipt(Id, Store, PurchaseDate, allPurchases, allChanges);
  }

  private Receipt ApplyEvent(Fact fact)
  {
    return fact switch
    {
      ReceiptCreated receiptCreated => Apply(receiptCreated),
      StoreCorrected storeCorrected => Apply(storeCorrected),
      PurchaseAdded purchaseAdded => Apply(purchaseAdded),
      _ => this,
    };
  }

  private Receipt Apply(ReceiptCreated fact)
  {
    return new Receipt(fact.Id, fact.Store, fact.PurchaseDate, Purchases, changes.ToList());
  }

  private Receipt Apply(StoreCorrected fact)
  {
    return new Receipt(Id, fact.Store, PurchaseDate, Purchases, changes.ToList());
  }

  private Receipt Apply(PurchaseAdded fact)
  {
    return new Receipt(Id, Store, PurchaseDate, Purchases.Append(fact.Purchase).ToList(), changes.ToList());
  }
}
