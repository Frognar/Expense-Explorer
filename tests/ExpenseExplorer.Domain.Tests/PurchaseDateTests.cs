namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Exceptions;
using ExpenseExplorer.Domain.ValueObjects;
using ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Dates;

public class PurchaseDateTests
{
  [Property(Arbitrary = [typeof(NonFutureDateOnlyGenerator)])]
  public void SetsDate(DateOnly date)
  {
    PurchaseDate purchaseDate = CreatePurchaseDate(date);
    purchaseDate.Date.Should().Be(date);
  }

  [Property(Arbitrary = [typeof(FutureDateOnlyGenerator)])]
  public void ThrowsExceptionWhenDateIsInTheFuture(DateOnly date)
  {
    Action act = () => _ = CreatePurchaseDate(date);
    act.Should().Throw<FutureDateException>();
  }

  [Fact]
  public void CanCreateMinimumDate()
  {
    PurchaseDate.MinValue.Date.Should().Be(DateOnly.MinValue);
  }

  private static PurchaseDate CreatePurchaseDate(DateOnly date)
  {
    return PurchaseDate.Create(date, Today);
  }
}
