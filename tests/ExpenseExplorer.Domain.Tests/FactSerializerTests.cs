namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Incomes.Facts;

public class FactSerializerTests
{
  [Fact]
  public void ThrowsWhenSerializingUnknownFact()
  {
    Fact fact = new UnknownFact();

    Action act = () => FactSerializer.Serialize(fact);

    act.Should().Throw<UnreachableException>();
  }

  [Fact]
  public void ThrowsWhenDeserializingUnknownFact()
  {
    byte[] data = "{}"u8.ToArray();

    Action act = () => FactSerializer.Deserialize("UNKNOWN", data);

    act.Should().Throw<UnreachableException>();
  }

  [Fact]
  public void SerializeReceiptCreated()
  {
    DateOnly today = new(2000, 1, 1);
    Fact fact = new ReceiptCreated("id", "store", today, today);

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
  public void SerializeStoreCorrected()
  {
    Fact fact = new StoreCorrected("id", "store");

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

  [Fact]
  public void SerializePurchaseDateChanged()
  {
    DateOnly today = new DateOnly(2001, 1, 1);
    DateOnly newDate = today.AddYears(-1);
    Fact fact = new PurchaseDateChanged("id", newDate, today);

    byte[] data = FactSerializer.Serialize(fact);

    data.Should()
      .BeEquivalentTo("{\"ReceiptId\":\"id\",\"PurchaseDate\":\"2000-01-01\",\"RequestedDate\":\"2001-01-01\"}"u8.ToArray());
  }

  [Fact]
  public void DeserializePurchaseDateChanged()
  {
    byte[] data = "{\"ReceiptId\":\"id\",\"PurchaseDate\":\"2000-01-01\",\"RequestedDate\":\"2001-01-01\"}"u8.ToArray();

    Fact fact = FactSerializer.Deserialize(FactTypes.PurchaseDateChangedFactType, data);

    fact.Should().BeOfType<PurchaseDateChanged>();
  }

  [Fact]
  public void SerializePurchaseAdded()
  {
    Fact fact = new PurchaseAdded("id", "pId", "i", "c", 1, 1, 0, string.Empty);

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
  public void SerializePurchaseDetailsChanged()
  {
    Fact fact = new PurchaseDetailsChanged("id", "pId", "i", "c", 1, 1, 0, string.Empty);

    byte[] data = FactSerializer.Serialize(fact);

    data.Should()
      .BeEquivalentTo(
        "{\"ReceiptId\":\"id\",\"PurchaseId\":\"pId\",\"Item\":\"i\",\"Category\":\"c\",\"Quantity\":1,\"UnitPrice\":1,\"TotalDiscount\":0,\"Description\":\"\"}"u8
          .ToArray());
  }

  [Fact]
  public void DeserializePurchaseDetailsChanged()
  {
    byte[] data
      = "{\"ReceiptId\":\"id\",\"PurchaseId\":\"pId\",\"Item\":\"i\",\"Category\":\"c\",\"Quantity\":1,\"UnitPrice\":1,\"TotalDiscount\":0,\"Description\":\"\"}"u8
        .ToArray();

    Fact fact = FactSerializer.Deserialize(FactTypes.PurchaseDetailsChangedFactType, data);

    fact.Should().BeOfType<PurchaseDetailsChanged>();
  }

  [Fact]
  public void SerializePurchaseRemoved()
  {
    Fact fact = new PurchaseRemoved("id", "pId");

    byte[] data = FactSerializer.Serialize(fact);

    data.Should().BeEquivalentTo("{\"ReceiptId\":\"id\",\"PurchaseId\":\"pId\"}"u8.ToArray());
  }

  [Fact]
  public void DeserializePurchaseRemoved()
  {
    byte[] data = "{\"ReceiptId\":\"id\",\"PurchaseId\":\"pId\"}"u8.ToArray();

    Fact fact = FactSerializer.Deserialize(FactTypes.PurchaseRemovedFactType, data);

    fact.Should().BeOfType<PurchaseRemoved>();
  }

  [Fact]
  public void SerializeReceiptDeleted()
  {
    Fact fact = new ReceiptDeleted("id");

    byte[] data = FactSerializer.Serialize(fact);

    data.Should().BeEquivalentTo("{\"ReceiptId\":\"id\"}"u8.ToArray());
  }

  [Fact]
  public void DeserializeReceiptDeleted()
  {
    byte[] data = "{\"ReceiptId\":\"id\"}"u8.ToArray();

    Fact fact = FactSerializer.Deserialize(FactTypes.ReceiptDeletedFactType, data);

    fact.Should().BeOfType<ReceiptDeleted>();
  }

  [Fact]
  public void SerializeIncomeCreated()
  {
    Fact fact = new IncomeCreated("id", "s", 1, new DateOnly(2000, 1, 1), "c", "d", new DateOnly(2001, 1, 1));

    byte[] data = FactSerializer.Serialize(fact);

    data.Should()
      .BeEquivalentTo(
        "{\"IncomeId\":\"id\",\"Source\":\"s\",\"Amount\":1,\"ReceivedDate\":\"2000-01-01\",\"Category\":\"c\",\"Description\":\"d\",\"CreatedDate\":\"2001-01-01\"}"u8
          .ToArray());
  }

  [Fact]
  public void DeserializeIncomeCreated()
  {
    byte[] data
      = "{\"IncomeId\":\"id\",\"Source\":\"s\",\"Amount\":1,\"ReceivedDate\":\"2000-01-01\",\"Category\":\"c\",\"Description\":\"d\",\"CreatedDate\":\"2001-01-01\"}"u8
        .ToArray();

    Fact fact = FactSerializer.Deserialize(FactTypes.IncomeCreatedFactType, data);

    fact.Should().BeOfType<IncomeCreated>();
  }

  [Fact]
  public void SerializeIncomeSourceCorrected()
  {
    Fact fact = new SourceCorrected("id", "s");

    byte[] data = FactSerializer.Serialize(fact);

    data.Should().BeEquivalentTo("{\"IncomeId\":\"id\",\"Source\":\"s\"}"u8.ToArray());
  }

  [Fact]
  public void DeserializeIncomeSourceCorrected()
  {
    byte[] data = "{\"IncomeId\":\"id\",\"Source\":\"s\"}"u8.ToArray();

    Fact fact = FactSerializer.Deserialize(FactTypes.IncomeSourceCorrectedFactType, data);

    fact.Should().BeOfType<SourceCorrected>();
  }

  [Fact]
  public void SerializeIncomeAmountCorrected()
  {
    Fact fact = new AmountCorrected("id", 1);

    byte[] data = FactSerializer.Serialize(fact);

    data.Should().BeEquivalentTo("{\"IncomeId\":\"id\",\"Amount\":1}"u8.ToArray());
  }

  [Fact]
  public void DeserializeIncomeAmountCorrected()
  {
    byte[] data = "{\"IncomeId\":\"id\",\"Amount\":1}"u8.ToArray();

    Fact fact = FactSerializer.Deserialize(FactTypes.IncomeAmountCorrectedFactType, data);

    fact.Should().BeOfType<AmountCorrected>();
  }

  [Fact]
  public void SerializeIncomeCategoryCorrected()
  {
    Fact fact = new CategoryCorrected("id", "c");

    byte[] data = FactSerializer.Serialize(fact);

    data.Should().BeEquivalentTo("{\"IncomeId\":\"id\",\"Category\":\"c\"}"u8.ToArray());
  }

  [Fact]
  public void DeserializeIncomeCategoryCorrected()
  {
    byte[] data = "{\"IncomeId\":\"id\",\"Category\":\"c\"}"u8.ToArray();

    Fact fact = FactSerializer.Deserialize(FactTypes.IncomeCategoryCorrectedFactType, data);

    fact.Should().BeOfType<CategoryCorrected>();
  }

  [Fact]
  public void SerializeIncomeReceivedDateCorrected()
  {
    Fact fact = new ReceivedDateCorrected("id", new DateOnly(2000, 1, 1));

    byte[] data = FactSerializer.Serialize(fact);

    data.Should().BeEquivalentTo("{\"IncomeId\":\"id\",\"ReceivedDate\":\"2000-01-01\"}"u8.ToArray());
  }

  [Fact]
  public void DeserializeIncomeReceivedDateCorrected()
  {
    byte[] data = "{\"IncomeId\":\"id\",\"ReceivedDate\":\"2000-01-01\"}"u8.ToArray();

    Fact fact = FactSerializer.Deserialize(FactTypes.IncomeReceivedDateCorrectedFactType, data);

    fact.Should().BeOfType<ReceivedDateCorrected>();
  }

  [Fact]
  public void SerializeIncomeDescriptionCorrected()
  {
    Fact fact = new DescriptionCorrected("id", "d");

    byte[] data = FactSerializer.Serialize(fact);

    data.Should().BeEquivalentTo("{\"IncomeId\":\"id\",\"Description\":\"d\"}"u8.ToArray());
  }

  [Fact]
  public void DeserializeIncomeDescriptionCorrected()
  {
    byte[] data = "{\"IncomeId\":\"id\",\"Description\":\"d\"}"u8.ToArray();

    Fact fact = FactSerializer.Deserialize(FactTypes.IncomeDescriptionCorrectedFactType, data);

    fact.Should().BeOfType<DescriptionCorrected>();
  }

  [Fact]
  public void SerializeIncomeDeleted()
  {
    Fact fact = new IncomeDeleted("id");

    byte[] data = FactSerializer.Serialize(fact);

    data.Should().BeEquivalentTo("{\"IncomeId\":\"id\"}"u8.ToArray());
  }

  [Fact]
  public void DeserializeIncomeDeleted()
  {
    byte[] data = "{\"IncomeId\":\"id\"}"u8.ToArray();

    Fact fact = FactSerializer.Deserialize(FactTypes.IncomeDeletedFactType, data);

    fact.Should().BeOfType<IncomeDeleted>();
  }

  private sealed record UnknownFact : Fact;
}
