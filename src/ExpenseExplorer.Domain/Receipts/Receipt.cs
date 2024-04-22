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
    Fact receiptCreated = ReceiptCreated.Create(id, store, purchaseDate, createdDate);
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
    Fact storeCorrected = StoreCorrected.Create(Id, store);
    List<Fact> allChanges = _unsavedChanges.Append(storeCorrected).ToList();
    return new Receipt(Id, store, PurchaseDate, Purchases, allChanges, Version);
  }

  public Receipt ChangePurchaseDate(PurchaseDate purchaseDate, DateOnly requestedDate)
  {
    Fact purchaseDateChanged = PurchaseDateChanged.Create(Id, purchaseDate, requestedDate);
    List<Fact> allChanges = _unsavedChanges.Append(purchaseDateChanged).ToList();
    return new Receipt(Id, Store, purchaseDate, Purchases, allChanges, Version);
  }

  public Receipt AddPurchase(Purchase purchase)
  {
    Fact purchaseAdded = PurchaseAdded.Create(Id, purchase);
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
    Id receiptId = Id.Create(fact.Id);
    Store store = Store.Create(fact.Store);
    PurchaseDate purchaseDate = PurchaseDate.Create(fact.PurchaseDate, fact.CreatedDate);
    return new Receipt(receiptId, store, purchaseDate, Purchases, _unsavedChanges.ToList(), Version);
  }

  private Receipt Apply(StoreCorrected fact)
  {
    Store store = Store.Create(fact.Store);
    return new Receipt(Id, store, PurchaseDate, Purchases, _unsavedChanges.ToList(), Version);
  }

  private Receipt Apply(PurchaseAdded fact)
  {
    Purchase purchase = Purchase.Create(
      Id.Create(fact.PurchaseId),
      Item.Create(fact.Item),
      Category.Create(fact.Category),
      Quantity.Create(fact.Quantity),
      Money.Create(fact.UnitPrice),
      Money.Create(fact.TotalDiscount),
      Description.Create(fact.Description));

    return new Receipt(
      Id,
      Store,
      PurchaseDate,
      Purchases.Append(purchase).ToList(),
      _unsavedChanges.ToList(),
      Version);
  }
}
