namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Receipts;
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
}
