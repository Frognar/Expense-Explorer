namespace ExpenseExplorer.Domain.Receipts;

using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Domain.ValueObjects;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

public sealed record Receipt
{
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

  public Id Id { get; }

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

  public static Result<Receipt> Recreate(IEnumerable<Fact> facts, Version version)
  {
    facts = facts.ToList();
    if (facts.FirstOrDefault() is ReceiptCreated receiptCreated)
    {
      return facts.Skip(1)
        .Aggregate(Apply(receiptCreated), (receipt, fact) => receipt.FlatMap(r => r.ApplyFact(fact)))
        .Map(r => r with { Version = version });
    }

    return Fail.OfType<Receipt>(
      Failure.Fatal(new ArgumentException("First fact must be a ReceiptCreated fact.", nameof(facts))));
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

  public Receipt RemovePurchase(Id purchaseId)
  {
    ArgumentNullException.ThrowIfNull(purchaseId);
    Fact purchaseRemoved = PurchaseRemoved.Create(Id, purchaseId);
    return this with
    {
      Purchases = Purchases.Where(p => p.Id != purchaseId).ToList(),
      UnsavedChanges = UnsavedChanges.Append(purchaseRemoved).ToList(),
    };
  }

  public Receipt Delete()
  {
    return this with { UnsavedChanges = UnsavedChanges.Append(ReceiptDeleted.Create(Id)).ToList() };
  }

  public Receipt WithVersion(Version version)
  {
    return this with { Version = version };
  }

  private Result<Receipt> ApplyFact(Fact fact)
  {
    return fact switch
    {
      StoreCorrected storeCorrected => Apply(storeCorrected),
      PurchaseAdded purchaseAdded => Apply(purchaseAdded),
      PurchaseDateChanged purchaseDateChanged => Apply(purchaseDateChanged),
      PurchaseDetailsChanged purchaseDetailsChanged => Apply(purchaseDetailsChanged),
      PurchaseRemoved purchaseRemoved => Apply(purchaseRemoved),
      ReceiptDeleted _ => Fail.OfType<Receipt>(Failure.NotFound("Receipt has been deleted.", Id.Value)),
      _ => Fail.OfType<Receipt>(
        Failure.Fatal(new ArgumentException($"Unknown fact type: {fact.GetType()}", nameof(fact)))),
    };
  }

  private static Result<Receipt> Apply(ReceiptCreated receiptCreated)
  {
    return (
        from id in Id.TryCreate(receiptCreated.Id)
        from store in Store.TryCreate(receiptCreated.Store)
        from purchaseDate in PurchaseDate.TryCreate(receiptCreated.PurchaseDate, receiptCreated.CreatedDate)
        select new Receipt(id, store, purchaseDate, [], [], Version.New()))
      .ToResult(() => Failure.Fatal(new ArgumentException($"Failed to create receipt from {receiptCreated}.")));
  }

  private Result<Receipt> Apply(StoreCorrected fact)
  {
    return (
        from store in Store.TryCreate(fact.Store)
        select this with { Store = store })
      .ToResult(() => Failure.Fatal(new AggregateException($"Failed to recreate receipt from {fact}.")));
  }

  private Result<Receipt> Apply(PurchaseAdded fact)
  {
    return (
        from purchase in Purchase.TryCreate(
          Id.TryCreate(fact.PurchaseId),
          Item.TryCreate(fact.Item),
          Category.TryCreate(fact.Category),
          Quantity.TryCreate(fact.Quantity),
          Money.TryCreate(fact.UnitPrice),
          Money.TryCreate(fact.TotalDiscount),
          Description.TryCreate(fact.Description))
        select this with { Purchases = Purchases.Append(purchase).ToList() })
      .ToResult(() => Failure.Fatal(new AggregateException($"Failed to recreate receipt from {fact}.")));
  }

  private Result<Receipt> Apply(PurchaseDetailsChanged fact)
  {
    return (
        from purchase in Purchase.TryCreate(
          Id.TryCreate(fact.PurchaseId),
          Item.TryCreate(fact.Item),
          Category.TryCreate(fact.Category),
          Quantity.TryCreate(fact.Quantity),
          Money.TryCreate(fact.UnitPrice),
          Money.TryCreate(fact.TotalDiscount),
          Description.TryCreate(fact.Description))
        select this with { Purchases = Purchases.Select(p => p.Id == purchase.Id ? purchase : p).ToList() })
      .ToResult(() => Failure.Fatal(new AggregateException($"Failed to recreate receipt from {fact}.")));
  }

  private Result<Receipt> Apply(PurchaseRemoved fact)
  {
    return (
        from purchaseId in Id.TryCreate(fact.PurchaseId)
        select this with { Purchases = Purchases.Where(p => p.Id != purchaseId).ToList() })
      .ToResult(() => Failure.Fatal(new AggregateException($"Failed to recreate receipt from {fact}.")));
  }

  private Result<Receipt> Apply(PurchaseDateChanged fact)
  {
    return (
        from purchaseDate in PurchaseDate.TryCreate(fact.PurchaseDate, fact.RequestedDate)
        select this with { PurchaseDate = purchaseDate })
      .ToResult(() => Failure.Fatal(new ArgumentException($"Failed to recreate receipt from {fact}.")));
  }
}
