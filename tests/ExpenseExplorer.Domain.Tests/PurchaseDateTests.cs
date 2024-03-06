using ExpenseExplorer.Domain.ValueObjects;

namespace ExpenseExplorer.Domain.Tests;

public class PurchaseDateTests {
  [Property(Arbitrary = [typeof(NonFutureDateOnlyGenerator)])]
  public void SetsDate(DateOnly date) {
    PurchaseDate purchaseDate = PurchaseDate.Create(date, todayDateOnly);
    purchaseDate.Date.Should().Be(date);
  }

  [Property(Arbitrary = [typeof(FutureDateOnlyGenerator)])]
  public void ThrowsExceptionWhenDateIsInTheFuture(DateOnly date) {
    Action act = () => _ = PurchaseDate.Create(date, todayDateOnly);
    act.Should().Throw<ArgumentException>();
  }
}
