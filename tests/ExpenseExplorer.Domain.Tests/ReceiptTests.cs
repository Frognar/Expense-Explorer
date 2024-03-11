namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;

public class ReceiptTests
{
  [Property(Arbitrary = [typeof(PurchaseDateGenerator), typeof(StoreGenerator)])]
  public void CanBeCreated(PurchaseDate purchaseDate, Store store)
  {
    Receipt receipt = Receipt.New(store, purchaseDate);
    receipt.Should().NotBeNull();
    receipt.Id.Should().NotBeNull();
    receipt.Store.Should().Be(store);
    receipt.PurchaseDate.Should().Be(purchaseDate);
  }

  [Property(Arbitrary = [typeof(PurchaseDateGenerator), typeof(StoreGenerator)])]
  public void ProducesReceiptCreatedEventWhenCreated(PurchaseDate purchaseDate, Store store)
  {
    Receipt receipt = Receipt.New(store, purchaseDate);
    receipt.UnsavedChanges.Count().Should().Be(1);
    ReceiptCreated receiptCreated = receipt.UnsavedChanges.OfType<ReceiptCreated>().Single();
    receiptCreated.Id.Should().Be(receipt.Id);
    receiptCreated.Store.Should().Be(receipt.Store);
    receiptCreated.PurchaseDate.Should().Be(receipt.PurchaseDate);
  }

  [Property(Arbitrary = [typeof(PurchaseDateGenerator), typeof(StoreGenerator)])]
  public void CanCorrectStore(PurchaseDate purchaseDate, Store store, Store newStore)
  {
    Receipt receipt = Receipt.New(store, purchaseDate);
    receipt = receipt.CorrectStore(newStore);
    receipt.Store.Should().Be(newStore);
  }

  [Property(Arbitrary = [typeof(PurchaseDateGenerator), typeof(StoreGenerator)])]
  public void ProducesStoreUpdatedEventWhenStoreUpdated(PurchaseDate purchaseDate, Store store, Store newStore)
  {
    Receipt receipt = Receipt.New(store, purchaseDate);
    receipt = receipt.CorrectStore(newStore);
    receipt.UnsavedChanges.Count().Should().Be(2);
    StoreCorrected storeCorrected = receipt.UnsavedChanges.OfType<StoreCorrected>().Single();
    storeCorrected.Id.Should().Be(receipt.Id);
    storeCorrected.Store.Should().Be(receipt.Store);
  }

  [Property(Arbitrary = [typeof(PurchaseDateGenerator), typeof(StoreGenerator)])]
  public void HasNoUnsavedChangesAfterClearingChanges(PurchaseDate purchaseDate, Store store, Store newStore)
  {
    Receipt.New(store, purchaseDate)
      .CorrectStore(newStore)
      .ClearChanges()
      .UnsavedChanges
      .Should()
      .BeEmpty();
  }
}
