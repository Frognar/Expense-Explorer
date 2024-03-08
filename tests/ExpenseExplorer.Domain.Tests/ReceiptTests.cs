namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;

public class ReceiptTests
{
  [Property(Arbitrary = [typeof(NonFutureDateOnlyGenerator), typeof(NonEmptyStringGenerator)])]
  public void CanBeCreated(DateOnly purchaseDate, string store)
  {
    Receipt receipt = Receipt.New(Store.Create(store), PurchaseDate.Create(purchaseDate, todayDateOnly));
    receipt.Should().NotBeNull();
    receipt.Id.Should().NotBeNull();
    receipt.Store.Should().Be(Store.Create(store));
    receipt.PurchaseDate.Should().Be(PurchaseDate.Create(purchaseDate, todayDateOnly));
  }

  [Property(Arbitrary = [typeof(NonFutureDateOnlyGenerator), typeof(NonEmptyStringGenerator)])]
  public void ProducesReceiptCreatedEventWhenCreated(DateOnly purchaseDate, string store)
  {
    Receipt receipt = Receipt.New(Store.Create(store), PurchaseDate.Create(purchaseDate, todayDateOnly));
    receipt.UnsavedChanges.Count().Should().Be(1);
    ReceiptCreated receiptCreated = receipt.UnsavedChanges.OfType<ReceiptCreated>().Single();
    receiptCreated.Id.Should().Be(receipt.Id);
    receiptCreated.Store.Should().Be(receipt.Store);
    receiptCreated.PurchaseDate.Should().Be(receipt.PurchaseDate);
  }
}
