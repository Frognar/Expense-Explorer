namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Events;
using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;

public class EventTypesTests
{
  [Fact]
  public void GetTypeForReceiptCreated()
  {
    Fact fact = new ReceiptCreated(
      Id.Unique(),
      Store.Create("store"),
      PurchaseDate.Create(new DateOnly(2000, 1, 1), TodayDateOnly),
      TodayDateOnly);

    AssertEventType(fact, EventTypes.ReceiptCreatedEventType);
  }

  [Fact]
  public void GetTypeForPurchaseAdded()
  {
    Fact fact = new PurchaseAdded(
      Id.Unique(),
      Purchase.Create(
        Item.Create("i"),
        Category.Create("c"),
        Quantity.Create(1),
        Money.Create(1),
        Money.Zero,
        Description.Create(null)));

    AssertEventType(fact, EventTypes.PurchaseAddedEventType);
  }

  private static void AssertEventType(Fact fact, string expectedType)
  {
    string type = EventTypes.GetType(fact);
    type.Should().Be(expectedType);
  }
}
