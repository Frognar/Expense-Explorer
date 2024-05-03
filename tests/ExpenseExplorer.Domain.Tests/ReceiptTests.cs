namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.Tests.TestData;
using FunctionalCore.Failures;
using FunctionalCore.Monads;

public class ReceiptTests
{
  [Property(Arbitrary = [typeof(PurchaseDateGenerator), typeof(StoreGenerator)])]
  public void CanBeCreated(PurchaseDate purchaseDate, Store store)
  {
    Receipt receipt = Receipt.New(store, purchaseDate, purchaseDate.Date);
    receipt.Should().NotBeNull();
    receipt.Id.Should().NotBeNull();
    receipt.Store.Should().Be(store);
    receipt.PurchaseDate.Should().Be(purchaseDate);
    receipt.Version.Should().Be(Version.Create(ulong.MaxValue));
  }

  [Property(Arbitrary = [typeof(PurchaseDateGenerator), typeof(StoreGenerator)])]
  public void ProducesReceiptCreatedFactWhenCreated(PurchaseDate purchaseDate, Store store)
  {
    Receipt receipt = Receipt.New(store, purchaseDate, purchaseDate.Date);
    receipt.UnsavedChanges.Count().Should().Be(1);
    ReceiptCreated receiptCreated = receipt.UnsavedChanges.OfType<ReceiptCreated>().Single();
    receiptCreated.Id.Should().Be(receipt.Id.Value);
    receiptCreated.Store.Should().Be(receipt.Store.Name);
    receiptCreated.PurchaseDate.Should().Be(receipt.PurchaseDate.Date);
  }

  [Property(Arbitrary = [typeof(ReceiptGenerator), typeof(StoreGenerator)])]
  public void CanCorrectStore(Receipt receipt, Store newStore)
  {
    receipt
      .CorrectStore(newStore)
      .Store
      .Should()
      .Be(newStore);
  }

  [Property(Arbitrary = [typeof(ReceiptGenerator), typeof(StoreGenerator)])]
  public void ProducesStoreUpdatedFactWhenStoreUpdated(Receipt receipt, Store newStore)
  {
    receipt = receipt.CorrectStore(newStore);
    StoreCorrected storeCorrected = receipt.UnsavedChanges.OfType<StoreCorrected>().Single();
    storeCorrected.ReceiptId.Should().Be(receipt.Id.Value);
    storeCorrected.Store.Should().Be(receipt.Store.Name);
  }

  [Property(Arbitrary = [typeof(ReceiptGenerator), typeof(StoreGenerator)])]
  public void HasNoUnsavedChangesAfterClearingChanges(Receipt receipt, Store newStore)
  {
    receipt
      .CorrectStore(newStore)
      .ClearChanges()
      .UnsavedChanges
      .Should()
      .BeEmpty();
  }

  [Property(Arbitrary = [typeof(ReceiptGenerator), typeof(PurchaseGenerator)])]
  public void CanAddPurchase(Receipt receipt, Purchase purchase)
  {
    receipt
      .AddPurchase(purchase)
      .Purchases
      .Should()
      .Contain(purchase);
  }

  [Property(Arbitrary = [typeof(ReceiptGenerator), typeof(PurchaseGenerator)])]
  public void ProducesPurchaseAddedFactWhenPurchaseAdded(Receipt receipt, Purchase purchase)
  {
    receipt = receipt.AddPurchase(purchase);
    PurchaseAdded purchaseAdded = receipt.UnsavedChanges.OfType<PurchaseAdded>().Single();
    purchaseAdded.ReceiptId.Should().Be(receipt.Id.Value);
    Purchase last = receipt.Purchases.Last();
    purchaseAdded.PurchaseId.Should().Be(last.Id.Value);
    purchaseAdded.Item.Should().Be(last.Item.Name);
    purchaseAdded.Category.Should().Be(last.Category.Name);
    purchaseAdded.Quantity.Should().Be(last.Quantity.Value);
    purchaseAdded.UnitPrice.Should().Be(last.UnitPrice.Value);
    purchaseAdded.TotalDiscount.Should().Be(last.TotalDiscount.Value);
    purchaseAdded.Description.Should().Be(last.Description.Value);
  }

  [Property(Arbitrary = [typeof(ReceiptGenerator), typeof(PurchaseDateGenerator)])]
  public void CanChangePurchaseDate(Receipt receipt, PurchaseDate newPurchaseDate)
  {
    receipt
      .ChangePurchaseDate(newPurchaseDate, newPurchaseDate.Date)
      .PurchaseDate
      .Should()
      .Be(newPurchaseDate);
  }

  [Property(Arbitrary = [typeof(ReceiptGenerator), typeof(PurchaseDateGenerator)])]
  public void ProducesPurchaseDateChangedFactWhenPurchaseDateChanged(Receipt receipt, PurchaseDate newPurchaseDate)
  {
    receipt = receipt.ChangePurchaseDate(newPurchaseDate, newPurchaseDate.Date);
    PurchaseDateChanged purchaseDateChanged = receipt.UnsavedChanges.OfType<PurchaseDateChanged>().Single();
    purchaseDateChanged.ReceiptId.Should().Be(receipt.Id.Value);
    purchaseDateChanged.PurchaseDate.Should().Be(newPurchaseDate.Date);
  }

  [Property(Arbitrary = [typeof(PurchaseGenerator)])]
  public void CanUpdatePurchaseDetails(Purchase purchase)
  {
    DateOnly today = new DateOnly(2000, 1, 1);
    List<Fact> facts =
    [
      new ReceiptCreated("id", "store", today, today),
      new PurchaseAdded("id", "pId", "i", "c", 1, 1, 0, "d"),
    ];

    purchase = purchase with { Id = Id.Create("pId") };
    Result<Receipt> resultOfReceipt = Receipt.Recreate(facts, Version.Create((ulong)(facts.Count - 1)));
    Receipt receipt = resultOfReceipt.Match(_ => throw new UnreachableException(), r => r);

    receipt = receipt.UpdatePurchaseDetails(purchase);

    var purchaseFromReceipt = receipt.Purchases.First();
    purchaseFromReceipt.Item.Should().Be(purchase.Item);
    purchaseFromReceipt.Category.Should().Be(purchase.Category);
    purchaseFromReceipt.Quantity.Should().Be(purchase.Quantity);
    purchaseFromReceipt.UnitPrice.Should().Be(purchase.UnitPrice);
    purchaseFromReceipt.TotalDiscount.Should().Be(purchase.TotalDiscount);
    purchaseFromReceipt.Description.Should().Be(purchase.Description);
  }

  [Property(Arbitrary = [typeof(PurchaseGenerator)])]
  public void ProducesPurchaseDetailsChangedFactWhenUpdatePurchaseDetails(Purchase purchase)
  {
    DateOnly today = new DateOnly(2000, 1, 1);
    List<Fact> facts =
    [
      new ReceiptCreated("id", "store", today, today),
      new PurchaseAdded("id", "pId", "i", "c", 1, 1, 0, "d"),
    ];

    purchase = purchase with { Id = Id.Create("pId") };
    Result<Receipt> resultOfReceipt = Receipt.Recreate(facts, Version.Create((ulong)(facts.Count - 1)));
    Receipt receipt = resultOfReceipt.Match(_ => throw new UnreachableException(), r => r);

    receipt = receipt.UpdatePurchaseDetails(purchase);

    PurchaseDetailsChanged fact = receipt.UnsavedChanges.OfType<PurchaseDetailsChanged>().Single();
    fact.ReceiptId.Should().Be(receipt.Id.Value);
    fact.PurchaseId.Should().Be(purchase.Id.Value);
    fact.Item.Should().Be(purchase.Item.Name);
    fact.Category.Should().Be(purchase.Category.Name);
    fact.Quantity.Should().Be(purchase.Quantity.Value);
    fact.UnitPrice.Should().Be(purchase.UnitPrice.Value);
    fact.TotalDiscount.Should().Be(purchase.TotalDiscount.Value);
    fact.Description.Should().Be(purchase.Description.Value);
  }

  [Fact]
  public void CanBeRecreatedFromFacts()
  {
    DateOnly today = new DateOnly(2000, 1, 1);
    List<Fact> facts =
    [
      new ReceiptCreated("id", "store", today, today),
      new PurchaseAdded("id", "pId", "i", "c", 1, 1, 0, "d"),
      new StoreCorrected("id", "newStore"),
      new PurchaseDateChanged("id", today.AddDays(-1), today),
      new PurchaseDetailsChanged("id", "pId", "it", "ca", 2, 2, 1, "de")
    ];

    Result<Receipt> resultOfReceipt = Receipt.Recreate(facts, Version.Create((ulong)(facts.Count - 1)));
    Receipt receipt = resultOfReceipt.Match(_ => throw new UnreachableException(), r => r);

    receipt.Id.Value.Should().Be("id");
    receipt.Store.Name.Should().Be("newStore");
    receipt.PurchaseDate.Date.Should().Be(new DateOnly(1999, 12, 31));
    receipt.Version.Value.Should().Be((ulong)(facts.Count - 1));
    Purchase purchase = receipt.Purchases.Single();
    purchase.Id.Value.Should().Be("pId");
    purchase.Item.Name.Should().Be("it");
    purchase.Category.Name.Should().Be("ca");
    purchase.Quantity.Value.Should().Be(2);
    purchase.UnitPrice.Value.Should().Be(2);
    purchase.TotalDiscount.Value.Should().Be(1);
    purchase.Description.Value.Should().Be("de");
  }

  [Fact]
  public void HasNoUnsavedChangesWhenRecreatedFromFacts()
  {
    DateOnly today = new DateOnly(2000, 1, 1);
    List<Fact> facts =
    [
      new ReceiptCreated("id", "store", today, today),
      new PurchaseAdded("id", "pId", "i", "c", 1, 1, 0, "d"),
      new StoreCorrected("id", "newStore"),
      new PurchaseDateChanged("id", today.AddDays(-1), today),
      new PurchaseDetailsChanged("id", "pId", "it", "ca", 2, 2, 1, "de")
    ];

    Result<Receipt> resultOfReceipt = Receipt.Recreate(facts, Version.Create((ulong)(facts.Count - 1)));
    Receipt receipt = resultOfReceipt.Match(_ => throw new UnreachableException(), r => r);

    receipt.UnsavedChanges.Should().BeEmpty();
  }

  [Fact]
  public void ReturnsFailureWhenRecreatedWithUnsupportedFact()
  {
    Fact unknown = new UnknownFact();

    Result<Receipt> resultOfReceipt = Receipt.Recreate([unknown], Version.Create(0UL));
    Failure failure = resultOfReceipt.Match(f => f, _ => throw new UnreachableException());

    failure.Should().NotBeNull();
  }

  [Fact]
  public void ReturnsFailureWhenRecreatedWithoutReceiptCreatedFact()
  {
    List<Fact> facts =
    [
      new PurchaseAdded("id", "pId", "i", "c", 1, 1, 0, "d"),
    ];

    Result<Receipt> resultOfReceipt = Receipt.Recreate(facts, Version.Create(0UL));
    Failure failure = resultOfReceipt.Match(f => f, _ => throw new UnreachableException());

    failure.Should().NotBeNull();
  }

  [Fact]
  public void ReturnsFailureWhenRecreatedWithMultipleReceiptCreatedFacts()
  {
    DateOnly today = new DateOnly(2000, 1, 1);
    List<Fact> facts =
    [
      new ReceiptCreated("id", "store", today, today),
      new ReceiptCreated("id", "store", today, today),
    ];

    Result<Receipt> resultOfReceipt = Receipt.Recreate(facts, Version.Create(0UL));
    Failure failure = resultOfReceipt.Match(f => f, _ => throw new UnreachableException());

    failure.Should().NotBeNull();
  }

  [Theory]
  [ClassData(typeof(ReceiptCorruptedFactsForRecreate))]
  public void ReturnsFailureWhenRecreatedWithCorruptedFact(Fact[] facts)
  {
    Result<Receipt> resultOfReceipt = Receipt.Recreate(facts, Version.Create(0UL));
    Failure failure = resultOfReceipt.Match(f => f, _ => throw new UnreachableException());

    failure.Should().NotBeNull();
  }

  private sealed record UnknownFact : Fact;
}
