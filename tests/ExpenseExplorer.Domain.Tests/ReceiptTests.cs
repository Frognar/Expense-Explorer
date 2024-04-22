namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Domain.ValueObjects;

public class ReceiptTests
{
  [Property(Arbitrary = [typeof(PurchaseDateGenerator), typeof(StoreGenerator)])]
  public void CanBeCreated(PurchaseDate purchaseDate, Store store)
  {
    Receipt receipt = Receipt.New(store, purchaseDate, TodayDateOnly);
    receipt.Should().NotBeNull();
    receipt.Id.Should().NotBeNull();
    receipt.Store.Should().Be(store);
    receipt.PurchaseDate.Should().Be(purchaseDate);
    receipt.Version.Should().Be(Version.Create(ulong.MaxValue));
  }

  [Property(Arbitrary = [typeof(PurchaseDateGenerator), typeof(StoreGenerator)])]
  public void ProducesReceiptCreatedFactWhenCreated(PurchaseDate purchaseDate, Store store)
  {
    Receipt receipt = Receipt.New(store, purchaseDate, TodayDateOnly);
    receipt.UnsavedChanges.Count().Should().Be(1);
    ReceiptCreated receiptCreated = receipt.UnsavedChanges.OfType<ReceiptCreated>().Single();
    receiptCreated.Id.Should().Be(receipt.Id);
    receiptCreated.Store.Should().Be(receipt.Store);
    receiptCreated.PurchaseDate.Should().Be(receipt.PurchaseDate);
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
    storeCorrected.ReceiptId.Should().Be(receipt.Id);
    storeCorrected.Store.Should().Be(receipt.Store);
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
    purchaseAdded.ReceiptId.Should().Be(receipt.Id);
    purchaseAdded.Purchase.Should().Be(receipt.Purchases.Last());
  }

  [Property(Arbitrary = [typeof(ReceiptGenerator), typeof(PurchaseDateGenerator)])]
  public void CanChangePurchaseDate(Receipt receipt, PurchaseDate newPurchaseDate)
  {
    receipt
      .ChangePurchaseDate(newPurchaseDate)
      .PurchaseDate
      .Should()
      .Be(newPurchaseDate);
  }

  [Property(Arbitrary = [typeof(PurchaseDateGenerator), typeof(StoreGenerator), typeof(PurchaseGenerator)])]
  public void CanBeRecreatedFromFacts(Store store, PurchaseDate purchaseDate, Purchase purchase, Store newStore)
  {
    Id receiptId = Id.Unique();
    List<Fact> facts =
    [
      new ReceiptCreated(receiptId, store, purchaseDate, TodayDateOnly),
      new PurchaseAdded(receiptId, purchase),
      new StoreCorrected(receiptId, newStore)
    ];

    Receipt receipt = Receipt.Recreate(facts, Version.Create((ulong)(facts.Count - 1)));
    receipt.Id.Should().Be(receiptId);
    receipt.Store.Should().Be(newStore);
    receipt.PurchaseDate.Should().Be(purchaseDate);
    receipt.Purchases.Should().Contain(purchase);
    receipt.Version.Should().Be(Version.Create((ulong)(facts.Count - 1)));
  }

  [Property(Arbitrary = [typeof(PurchaseDateGenerator), typeof(StoreGenerator), typeof(PurchaseGenerator)])]
  public void HasNoUnsavedChangesWhenRecreatedFromFacts(
    Store store,
    PurchaseDate purchaseDate,
    Purchase purchase,
    Store newStore)
  {
    Id receiptId = Id.Unique();
    List<Fact> facts =
    [
      new ReceiptCreated(receiptId, store, purchaseDate, TodayDateOnly),
      new PurchaseAdded(receiptId, purchase),
      new StoreCorrected(receiptId, newStore)
    ];

    Receipt receipt = Receipt.Recreate(facts, Version.Create((ulong)(facts.Count - 1)));
    receipt.UnsavedChanges.Should().BeEmpty();
  }
}
