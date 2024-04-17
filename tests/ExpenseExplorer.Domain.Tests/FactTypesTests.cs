namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Domain.ValueObjects;

public class FactTypesTests
{
  [Fact]
  public void GetTypeForReceiptCreated()
  {
    Fact fact = new ReceiptCreated(
      Id.Unique(),
      Store.Create("store"),
      PurchaseDate.Create(new DateOnly(2000, 1, 1), TodayDateOnly),
      TodayDateOnly);

    AssertFactType(fact, FactTypes.ReceiptCreatedFactType);
  }

  [Fact]
  public void GetTypeForPurchaseAdded()
  {
    Fact fact = new PurchaseAdded(
      Id.Unique(),
      Purchase.Create(
        Id.Unique(),
        Item.Create("i"),
        Category.Create("c"),
        Quantity.Create(1),
        Money.Create(1),
        Money.Zero,
        Description.Create(null)));

    AssertFactType(fact, FactTypes.PurchaseAddedFactType);
  }

  [Fact]
  public void GetTypeForStoreCorrected()
  {
    Fact fact = new StoreCorrected(Id.Unique(), Store.Create("store"));
    AssertFactType(fact, FactTypes.StoreCorrectedFactType);
  }

  private static void AssertFactType(Fact fact, string expectedType)
  {
    string type = FactTypes.GetFactType(fact);
    type.Should().Be(expectedType);
  }
}
