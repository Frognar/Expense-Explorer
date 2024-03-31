namespace ExpenseExplorer.Domain.Receipts;

using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Domain.ValueObjects;

public class Receipt
{
  private static readonly Receipt _empty = new();
  private readonly IEnumerable<Fact> _unsavedChanges;

  private Receipt()
    : this(Id.Create("[EMPTY]"), Store.Create("[EMPTY]"), PurchaseDate.MinValue, [], [], Version.Create(0))
  {
  }

  private Receipt(
    Id id,
    Store store,
    PurchaseDate purchaseDate,
    ICollection<Purchase> purchases,
    IEnumerable<Fact> unsavedChanges,
    Version version)
  {
    Id = id;
    Store = store;
    PurchaseDate = purchaseDate;
    Purchases = purchases;
    _unsavedChanges = unsavedChanges;
    Version = version;
  }

  public Id Id { get; }

  public Store Store { get; }

  public PurchaseDate PurchaseDate { get; }

  public ICollection<Purchase> Purchases { get; }

  public IEnumerable<Fact> UnsavedChanges => _unsavedChanges;

  public Version Version { get; }

  public static Receipt New(Store store, PurchaseDate purchaseDate, DateOnly createdDate)
  {
    Id id = Id.Unique();
    Fact receiptCreated = new ReceiptCreated(id, store, purchaseDate, createdDate);
    return new Receipt(id, store, purchaseDate, [], [receiptCreated], Version.New());
  }

  public static Receipt Recreate(IEnumerable<Fact> facts, Version version)
  {
    Receipt receipt = facts.Aggregate(_empty, (receipt, fact) => receipt.ApplyFact(fact));
    return receipt.WithVersion(version);
  }

  public Receipt ClearChanges()
  {
    return new Receipt(Id, Store, PurchaseDate, Purchases, [], Version);
  }

  public Receipt CorrectStore(Store store)
  {
    Fact storeCorrected = new StoreCorrected(Id, store);
    List<Fact> allChanges = _unsavedChanges.Append(storeCorrected).ToList();
    return new Receipt(Id, store, PurchaseDate, Purchases, allChanges, Version);
  }

  public Receipt AddPurchase(Purchase purchase)
  {
    Fact purchaseAdded = new PurchaseAdded(Id, purchase);
    List<Fact> allChanges = _unsavedChanges.Append(purchaseAdded).ToList();
    List<Purchase> allPurchases = Purchases.Append(purchase).ToList();
    return new Receipt(Id, Store, PurchaseDate, allPurchases, allChanges, Version);
  }

  public Receipt WithVersion(Version version)
  {
    return new Receipt(Id, Store, PurchaseDate, Purchases, _unsavedChanges, version);
  }

  private Receipt ApplyFact(Fact fact)
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
    return new Receipt(fact.Id, fact.Store, fact.PurchaseDate, Purchases, _unsavedChanges.ToList(), Version);
  }

  private Receipt Apply(StoreCorrected fact)
  {
    return new Receipt(Id, fact.Store, PurchaseDate, Purchases, _unsavedChanges.ToList(), Version);
  }

  private Receipt Apply(PurchaseAdded fact)
  {
    return new Receipt(
      Id,
      Store,
      PurchaseDate,
      Purchases.Append(fact.Purchase).ToList(),
      _unsavedChanges.ToList(),
      Version);
  }
}
