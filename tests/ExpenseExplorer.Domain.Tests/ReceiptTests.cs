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

  [Property(Arbitrary = [typeof(PurchaseDateGenerator), typeof(StoreGenerator), typeof(PurchaseGenerator)])]
  public void CanBeRecreatedFromFacts(
    Store store,
    PurchaseDate purchaseDate,
    Purchase purchase,
    Store newStore,
    PurchaseDate newPurchaseDate)
  {
    Id receiptId = Id.Unique();
    List<Fact> facts =
    [
      ReceiptCreated.Create(receiptId, store, purchaseDate, TodayDateOnly),
      PurchaseAdded.Create(receiptId, purchase),
      StoreCorrected.Create(receiptId, newStore),
      PurchaseDateChanged.Create(receiptId, newPurchaseDate, TodayDateOnly)
    ];

    Receipt receipt = Receipt.Recreate(facts, Version.Create((ulong)(facts.Count - 1)));
    receipt.Id.Should().Be(receiptId);
    receipt.Store.Should().Be(newStore);
    receipt.PurchaseDate.Should().Be(newPurchaseDate);
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
      ReceiptCreated.Create(receiptId, store, purchaseDate, TodayDateOnly),
      PurchaseAdded.Create(receiptId, purchase),
      StoreCorrected.Create(receiptId, newStore)
    ];

    Receipt receipt = Receipt.Recreate(facts, Version.Create((ulong)(facts.Count - 1)));
    receipt.UnsavedChanges.Should().BeEmpty();
  }
}
