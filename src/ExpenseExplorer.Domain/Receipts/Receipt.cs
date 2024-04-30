namespace ExpenseExplorer.Domain.Receipts;

using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Domain.ValueObjects;

public sealed record Receipt
{
  private static readonly Receipt _empty = new();

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
    UnsavedChanges = unsavedChanges;
    Version = version;
  }

  public Id Id { get; private init; }

  public Store Store { get; private init; }

  public PurchaseDate PurchaseDate { get; private init; }

  public ICollection<Purchase> Purchases { get; private init; }

  public IEnumerable<Fact> UnsavedChanges { get; private init; }

  public Version Version { get; private init; }

  public static Receipt New(Store store, PurchaseDate purchaseDate, DateOnly createdDate)
  {
    Id id = Id.Unique();
    Fact receiptCreated = ReceiptCreated.Create(id, store, purchaseDate, createdDate);
    return new Receipt(id, store, purchaseDate, [], [receiptCreated], Version.New());
  }

  public static Receipt Recreate(IEnumerable<Fact> facts, Version version)
  {
    return facts.Aggregate(_empty, (receipt, fact) => receipt.ApplyFact(fact)) with { Version = version };
  }

  public Receipt ClearChanges()
  {
    return this with { UnsavedChanges = [] };
  }

  public Receipt CorrectStore(Store store)
  {
    Fact storeCorrected = StoreCorrected.Create(Id, store);
    return this with { Store = store, UnsavedChanges = UnsavedChanges.Append(storeCorrected).ToList() };
  }

  public Receipt ChangePurchaseDate(PurchaseDate purchaseDate, DateOnly requestedDate)
  {
    Fact purchaseDateChanged = PurchaseDateChanged.Create(Id, purchaseDate, requestedDate);
    return this with
    {
      PurchaseDate = purchaseDate, UnsavedChanges = UnsavedChanges.Append(purchaseDateChanged).ToList(),
    };
  }

  public Receipt AddPurchase(Purchase purchase)
  {
    Fact purchaseAdded = PurchaseAdded.Create(Id, purchase);
    return this with
    {
      Purchases = Purchases.Append(purchase).ToList(), UnsavedChanges = UnsavedChanges.Append(purchaseAdded).ToList(),
    };
  }

  public Receipt UpdatePurchaseDetails(Purchase purchase)
  {
    ArgumentNullException.ThrowIfNull(purchase);
    Fact purchaseDetailsChanged = PurchaseDetailsChanged.Create(Id, purchase);
    return this with
    {
      Purchases = Purchases.Select(p => p.Id == purchase.Id ? purchase : p).ToList(),
      UnsavedChanges = UnsavedChanges.Append(purchaseDetailsChanged).ToList(),
    };
  }

  public Receipt WithVersion(Version version)
  {
    return this with { Version = version };
  }

  private Receipt ApplyFact(Fact fact)
  {
    return fact switch
    {
      ReceiptCreated receiptCreated => Apply(receiptCreated),
      StoreCorrected storeCorrected => Apply(storeCorrected),
      PurchaseAdded purchaseAdded => Apply(purchaseAdded),
      PurchaseDateChanged purchaseDateChanged => Apply(purchaseDateChanged),
      PurchaseDetailsChanged purchaseDetailsChanged => Apply(purchaseDetailsChanged),
      _ => throw new ArgumentException($"Unknown fact type: {fact.GetType()}", nameof(fact)),
    };
  }

  private Receipt Apply(ReceiptCreated fact)
  {
    Id receiptId = Id.Create(fact.Id);
    Store store = Store.Create(fact.Store);
    PurchaseDate purchaseDate = PurchaseDate.Create(fact.PurchaseDate, fact.CreatedDate);
    return this with { Id = receiptId, Store = store, PurchaseDate = purchaseDate };
  }

  private Receipt Apply(StoreCorrected fact)
  {
    Store store = Store.Create(fact.Store);
    return this with { Store = store };
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

    return this with { Purchases = Purchases.Append(purchase).ToList() };
  }

  private Receipt Apply(PurchaseDetailsChanged fact)
  {
    Purchase purchase = Purchase.Create(
      Id.Create(fact.PurchaseId),
      Item.Create(fact.Item),
      Category.Create(fact.Category),
      Quantity.Create(fact.Quantity),
      Money.Create(fact.UnitPrice),
      Money.Create(fact.TotalDiscount),
      Description.Create(fact.Description));

    return this with { Purchases = Purchases.Select(p => p.Id == purchase.Id ? purchase : p).ToList() };
  }

  private Receipt Apply(PurchaseDateChanged fact)
  {
    PurchaseDate purchaseDate = PurchaseDate.Create(fact.PurchaseDate, fact.RequestedDate);
    return this with { PurchaseDate = purchaseDate };
  }
}
