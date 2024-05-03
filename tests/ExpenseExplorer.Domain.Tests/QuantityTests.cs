namespace ExpenseExplorer.Domain.Tests;

public class QuantityTests
{
  [Property(Arbitrary = [typeof(PositiveDecimalGenerator)])]
  public void ValueIsRoundToQuantityPrecision(decimal value)
  {
    Quantity.TryCreate(value)
      .Match(() => -1, q => q.Value)
      .Should()
      .Be(Math.Round(value, Quantity.Precision));
  }

  [Property(Arbitrary = [typeof(NonPositiveDecimalGenerator)])]
  public void ReturnsNoneWhenValueIsNotPositive(decimal value)
  {
    Quantity.TryCreate(value)
      .Match(() => -1, q => q.Value)
      .Should()
      .Be(-1);
  }
}
