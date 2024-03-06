using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Tests;

public class PurchaseDateTests {
  [Property(Arbitrary = [typeof(NonFutureDateOnlyGenerator)])]
  public void SetsDate(DateOnly date) {
    PurchaseDate purchaseDate = CreatePurchaseDate(date);
    purchaseDate.Date.Should().Be(date);
  }

  [Property(Arbitrary = [typeof(FutureDateOnlyGenerator)])]
  public void ThrowsExceptionWhenDateIsInTheFuture(DateOnly date) {
    Action act = () => _ = CreatePurchaseDate(date);
    act.Should().Throw<ArgumentException>();
  }

  private static PurchaseDate CreatePurchaseDate(DateOnly date) {
    return PurchaseDate.Create(date, todayDateOnly);
  }
}
