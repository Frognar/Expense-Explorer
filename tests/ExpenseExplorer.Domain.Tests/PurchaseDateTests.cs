namespace ExpenseExplorer.Domain.Tests;

public class PurchaseDateTests
{
  [Property(Arbitrary = [typeof(DateOnlyGenerator)])]
  public void SetsDate(DateOnly date)
  {
    PurchaseDate.TryCreate(date, date)
      .Match(() => DateOnly.MinValue, d => d.Date)
      .Should()
      .Be(date);
  }

  [Property(Arbitrary = [typeof(DateOnlyGenerator)])]
  public void ReturnsNoneWhenDateIsInTheFuture(DateOnly date)
  {
    PurchaseDate.TryCreate(date, date)
      .Match(() => DateOnly.MinValue, d => d.Date)
      .Should()
      .Be(DateOnly.MinValue);
  }

  [Fact]
  public void CanCreateMinimumDate()
  {
    PurchaseDate.MinValue.Date
      .Should()
      .Be(DateOnly.MinValue);
  }
}
