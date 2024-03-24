namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Events;
using ExpenseExplorer.Domain.Receipts.Events;
using ExpenseExplorer.Domain.ValueObjects;

public class EventSerializerTests
{
  [Fact]
  public void SerializeReceiptCreated()
  {
    Fact fact = new ReceiptCreated(
      Id.Create("id"),
      Store.Create("store"),
      PurchaseDate.Create(new DateOnly(2000, 1, 1), new DateOnly(2000, 1, 1)));

    byte[] data = EventSerializer.Serialize(fact);

    data.Should()
      .BeEquivalentTo(
        "{\"Id\":{\"Value\":\"id\"},\"Store\":{\"Name\":\"store\"},\"PurchaseDate\":{\"Date\":\"2000-01-01\"}}"u8
          .ToArray());
  }

  [Fact]
  public void DeserializeReceiptCreated()
  {
    byte[] data
      = "{\"Id\":{\"Value\":\"id\"},\"Store\":{\"Name\":\"store\"},\"PurchaseDate\":{\"Date\":\"2000-01-01\"}}"u8
        .ToArray();

    Fact fact = EventSerializer.Deserialize(EventTypes.ReceiptCreatedEventType, data);

    fact.Should().BeOfType<ReceiptCreated>();
  }

  [Fact]
  public void SerializePurchaseAdded()
  {
    Fact fact = new PurchaseAdded(
      Id.Create("id"),
      Purchase.Create(
        Item.Create("i"),
        Category.Create("c"),
        Quantity.Create(1),
        Money.Create(1),
        Money.Zero,
        Description.Create(null)));

    byte[] data = EventSerializer.Serialize(fact);

    data.Should()
      .BeEquivalentTo(
        "{\"ReceiptId\":{\"Value\":\"id\"},\"Purchase\":{\"Item\":{\"Name\":\"i\"},\"Category\":{\"Name\":\"c\"},\"Quantity\":{\"Value\":1},\"UnitPrice\":{\"Value\":1},\"TotalDiscount\":{\"Value\":0},\"Description\":{\"Value\":\"\"}}}"u8
          .ToArray());
  }
}
