namespace ExpenseExplorer.Domain.Tests;

public class NonFutureDateTests
{
  [Property(Arbitrary = [typeof(DateOnlyGenerator)])]
  public void SetsDate(DateOnly date)
  {
    NonFutureDate.TryCreate(date, date)
      .Match(() => DateOnly.MinValue, d => d.Date)
      .Should()
      .Be(date);
  }

  [Property(Arbitrary = [typeof(DateOnlyGenerator)])]
  public void ReturnsNoneWhenDateIsInTheFuture(DateOnly date)
  {
    NonFutureDate.TryCreate(date.AddDays(1), date)
      .Match(() => DateOnly.MinValue, d => d.Date)
      .Should()
      .Be(DateOnly.MinValue);
  }

  [Fact]
  public void CanCreateMinimumDate()
  {
    NonFutureDate.MinValue.Date
      .Should()
      .Be(DateOnly.MinValue);
  }
}
