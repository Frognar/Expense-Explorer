namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Receipts;
using ExpenseExplorer.Domain.ValueObjects;

public class ReceiptTests
{
  [Property(Arbitrary = [typeof(NonFutureDateOnlyGenerator), typeof(NonEmptyStringGenerator)])]
  public void CanBeCreated(DateOnly purchaseDate, string store)
  {
    Receipt receipt = new(Id.Unique(), Store.Create(store), PurchaseDate.Create(purchaseDate, todayDateOnly));
    receipt.Should().NotBeNull();
  }
}
