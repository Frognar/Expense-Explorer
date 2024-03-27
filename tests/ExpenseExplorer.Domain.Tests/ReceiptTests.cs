namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.Receipts.Events;
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
  }

  [Property(Arbitrary = [typeof(PurchaseDateGenerator), typeof(StoreGenerator)])]
  public void ProducesReceiptCreatedEventWhenCreated(PurchaseDate purchaseDate, Store store)
  {
    Receipt receipt = Receipt.New(store, purchaseDate, TodayDateOnly);
    receipt.UnsavedUnsavedChanges.Count().Should().Be(1);
    ReceiptCreated receiptCreated = receipt.UnsavedUnsavedChanges.OfType<ReceiptCreated>().Single();
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
  public void ProducesStoreUpdatedEventWhenStoreUpdated(Receipt receipt, Store newStore)
  {
    receipt = receipt.CorrectStore(newStore);
    StoreCorrected storeCorrected = receipt.UnsavedUnsavedChanges.OfType<StoreCorrected>().Single();
    storeCorrected.ReceiptId.Should().Be(receipt.Id);
    storeCorrected.Store.Should().Be(receipt.Store);
  }

  [Property(Arbitrary = [typeof(ReceiptGenerator), typeof(StoreGenerator)])]
  public void HasNoUnsavedChangesAfterClearingChanges(Receipt receipt, Store newStore)
  {
    receipt
      .CorrectStore(newStore)
      .ClearChanges()
      .UnsavedUnsavedChanges
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
  public void ProducesPurchaseAddedEventWhenPurchaseAdded(Receipt receipt, Purchase purchase)
  {
    receipt = receipt.AddPurchase(purchase);
    PurchaseAdded purchaseAdded = receipt.UnsavedUnsavedChanges.OfType<PurchaseAdded>().Single();
    purchaseAdded.ReceiptId.Should().Be(receipt.Id);
    purchaseAdded.Purchase.Should().Be(receipt.Purchases.Last());
  }

  [Property(Arbitrary = [typeof(PurchaseDateGenerator), typeof(StoreGenerator), typeof(PurchaseGenerator)])]
  public void CanBeRecreatedFromEvents(Store store, PurchaseDate purchaseDate, Purchase purchase, Store newStore)
  {
    Id receiptId = Id.Unique();
    IEnumerable<Fact> events = new List<Fact>
    {
      new ReceiptCreated(receiptId, store, purchaseDate, TodayDateOnly),
      new PurchaseAdded(receiptId, purchase),
      new StoreCorrected(receiptId, newStore),
    };

    Receipt receipt = Receipt.Recreate(events);
    receipt.Id.Should().Be(receiptId);
    receipt.Store.Should().Be(newStore);
    receipt.PurchaseDate.Should().Be(purchaseDate);
    receipt.Purchases.Should().Contain(purchase);
  }

  [Property(Arbitrary = [typeof(PurchaseDateGenerator), typeof(StoreGenerator), typeof(PurchaseGenerator)])]
  public void HasNoUnsavedChangesWhenRecreatedFromEvents(
    Store store,
    PurchaseDate purchaseDate,
    Purchase purchase,
    Store newStore)
  {
    Id receiptId = Id.Unique();
    IEnumerable<Fact> events = new List<Fact>
    {
      new ReceiptCreated(receiptId, store, purchaseDate, TodayDateOnly),
      new PurchaseAdded(receiptId, purchase),
      new StoreCorrected(receiptId, newStore),
    };

    Receipt receipt = Receipt.Recreate(events);
    receipt.UnsavedUnsavedChanges.Should().BeEmpty();
  }
}
