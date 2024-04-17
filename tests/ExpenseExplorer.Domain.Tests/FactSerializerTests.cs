namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Domain.ValueObjects;

public class FactSerializerTests
{
  [Fact]
  public void SerializeReceiptCreated()
  {
    Fact fact = new ReceiptCreated(
      Id.Create("id"),
      Store.Create("store"),
      PurchaseDate.Create(new DateOnly(2000, 1, 1), TodayDateOnly),
      TodayDateOnly);

    byte[] data = FactSerializer.Serialize(fact);

    data.Should()
      .BeEquivalentTo(
        "{\"Id\":\"id\",\"Store\":\"store\",\"PurchaseDate\":\"2000-01-01\",\"CreatedDate\":\"2000-01-01\"}"u8
          .ToArray());
  }

  [Fact]
  public void DeserializeReceiptCreated()
  {
    byte[] data
      = "{\"Id\":\"id\",\"Store\":\"store\",\"PurchaseDate\":\"2000-01-01\",\"CreatedDate\":\"2000-01-01\"}"u8
        .ToArray();

    Fact fact = FactSerializer.Deserialize(FactTypes.ReceiptCreatedFactType, data);

    fact.Should().BeOfType<ReceiptCreated>();
  }

  [Fact]
  public void SerializePurchaseAdded()
  {
    Fact fact = new PurchaseAdded(
      Id.Create("id"),
      Purchase.Create(
        Id.Create("pId"),
        Item.Create("i"),
        Category.Create("c"),
        Quantity.Create(1),
        Money.Create(1),
        Money.Zero,
        Description.Create(null)));

    byte[] data = FactSerializer.Serialize(fact);

    data.Should()
      .BeEquivalentTo(
        "{\"ReceiptId\":\"id\",\"PurchaseId\":\"pId\",\"Item\":\"i\",\"Category\":\"c\",\"Quantity\":1,\"UnitPrice\":1,\"TotalDiscount\":0,\"Description\":\"\"}"u8
          .ToArray());
  }

  [Fact]
  public void DeserializePurchaseAdded()
  {
    byte[] data
      = "{\"ReceiptId\":\"id\",\"PurchaseId\":\"pId\",\"Item\":\"i\",\"Category\":\"c\",\"Quantity\":1,\"UnitPrice\":1,\"TotalDiscount\":0,\"Description\":\"\"}"u8
        .ToArray();

    Fact fact = FactSerializer.Deserialize(FactTypes.PurchaseAddedFactType, data);

    fact.Should().BeOfType<PurchaseAdded>();
  }

  [Fact]
  public void SerializeStoreCorrected()
  {
    Fact fact = new StoreCorrected(Id.Create("id"), Store.Create("store"));

    byte[] data = FactSerializer.Serialize(fact);

    data.Should().BeEquivalentTo("{\"ReceiptId\":\"id\",\"Store\":\"store\"}"u8.ToArray());
  }

  [Fact]
  public void DeserializeStoreCorrected()
  {
    byte[] data = "{\"ReceiptId\":\"id\",\"Store\":\"store\"}"u8.ToArray();

    Fact fact = FactSerializer.Deserialize(FactTypes.StoreCorrectedFactType, data);

    fact.Should().BeOfType<StoreCorrected>();
  }
}
