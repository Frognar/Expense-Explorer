namespace ExpenseExplorer.Domain.Tests;

public class PurchaseDateTests
{
  [Property(Arbitrary = [typeof(DateOnlyGenerator)])]
  public void SetsDate(DateOnly date)
  {
    PurchaseDate purchaseDate = PurchaseDate.Create(date, date);
    purchaseDate.Date.Should().Be(date);
  }

  [Property(Arbitrary = [typeof(DateOnlyGenerator)])]
  public void ThrowsExceptionWhenDateIsInTheFuture(DateOnly date)
  {
    Action act = () => _ = PurchaseDate.Create(date.AddDays(1), date);
    act.Should().Throw<FutureDateException>();
  }

  [Property(Arbitrary = [typeof(DateOnlyGenerator)])]
  public void SetsDateWithRecordSyntax(DateOnly date)
  {
    PurchaseDate purchaseDate = PurchaseDate.Create(date, date) with { Date = date.AddDays(-1) };
    purchaseDate.Date.Should().Be(date.AddDays(-1));
  }

  [Property(Arbitrary = [typeof(DateOnlyGenerator)])]
  public void ThrowsExceptionWhenDateIsInTheFutureWithRecordSyntax(DateOnly date)
  {
    Action act = () => _ = PurchaseDate.Create(date, date) with { Date = date.AddDays(1) };
    act.Should().Throw<FutureDateException>();
  }

  [Fact]
  public void CanCreateMinimumDate()
  {
    PurchaseDate.MinValue.Date.Should().Be(DateOnly.MinValue);
  }
}
