namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Facts;
using ExpenseExplorer.Domain.Receipts.Facts;
using ExpenseExplorer.Domain.ValueObjects;

public class FactTypesTests
{
  [Fact]
  public void GetTypeForReceiptCreated()
  {
    Fact fact = ReceiptCreated.Create(
      Id.Unique(),
      Store.Create("store"),
      PurchaseDate.Create(new DateOnly(2000, 1, 1), TodayDateOnly),
      TodayDateOnly);

    AssertFactType(fact, FactTypes.ReceiptCreatedFactType);
  }

  [Fact]
  public void GetTypeForPurchaseAdded()
  {
    Fact fact = PurchaseAdded.Create(
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
    Fact fact = StoreCorrected.Create(Id.Unique(), Store.Create("store"));
    AssertFactType(fact, FactTypes.StoreCorrectedFactType);
  }

  [Fact]
  public void GetTypeForPurchaseDateChanged()
  {
    DateOnly today = new DateOnly(2021, 1, 1);
    Fact fact = PurchaseDateChanged.Create(Id.Unique(), PurchaseDate.Create(today.AddDays(-1), today), today);
    AssertFactType(fact, FactTypes.PurchaseDateChangedFactType);
  }

  private static void AssertFactType(Fact fact, string expectedType)
  {
    string type = FactTypes.GetFactType(fact);
    type.Should().Be(expectedType);
  }
}
