namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Incomes.Facts;

public class FactTypesTests
{
  [Fact]
  public void ThrowsWhenGettingTypeForUnknownFact()
  {
    Fact fact = new UnknownFact();
    Action act = () => FactTypes.GetFactType(fact);
    act.Should().Throw<UnreachableException>();
  }

  [Fact]
  public void GetTypeForReceiptCreated()
  {
    DateOnly today = new(2000, 1, 1);
    Fact fact = new ReceiptCreated("id", "store", today, today);
    AssertFactType(fact, FactTypes.ReceiptCreatedFactType);
  }

  [Fact]
  public void GetTypeForStoreCorrected()
  {
    Fact fact = new StoreCorrected("id", "store");
    AssertFactType(fact, FactTypes.StoreCorrectedFactType);
  }

  [Fact]
  public void GetTypeForPurchaseDateChanged()
  {
    DateOnly today = new DateOnly(2021, 1, 1);
    Fact fact = new PurchaseDateChanged("id", today.AddDays(-1), today);
    AssertFactType(fact, FactTypes.PurchaseDateChangedFactType);
  }

  [Fact]
  public void GetTypeForPurchaseAdded()
  {
    Fact fact = new PurchaseAdded("id", "pid", "i", "c", 1, 1, 0, string.Empty);
    AssertFactType(fact, FactTypes.PurchaseAddedFactType);
  }

  [Fact]
  public void GetTypeForPurchaseDetailsChanged()
  {
    Fact fact = new PurchaseDetailsChanged("id", "pid", "i", "c", 1, 1, 0, string.Empty);
    AssertFactType(fact, FactTypes.PurchaseDetailsChangedFactType);
  }

  [Fact]
  public void GetTypeForPurchaseRemoved()
  {
    Fact fact = new PurchaseRemoved("id", "pid");
    AssertFactType(fact, FactTypes.PurchaseRemovedFactType);
  }

  [Fact]
  public void GetTypeForReceiptDeleted()
  {
    Fact fact = new ReceiptDeleted("id");
    AssertFactType(fact, FactTypes.ReceiptDeletedFactType);
  }

  [Fact]
  public void GetTypeForIncomeCreated()
  {
    Fact fact = new IncomeCreated("id", "s", 1, new DateOnly(2000, 1, 1), "c", "d", new DateOnly(2001, 1, 1));
    AssertFactType(fact, FactTypes.IncomeCreatedFactType);
  }

  private static void AssertFactType(Fact fact, string expectedType)
  {
    string type = FactTypes.GetFactType(fact);
    type.Should().Be(expectedType);
  }

  private sealed record UnknownFact : Fact;
}
