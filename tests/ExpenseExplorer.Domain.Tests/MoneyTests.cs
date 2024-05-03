namespace ExpenseExplorer.Domain.Tests;

public class MoneyTests
{
  [Property(Arbitrary = [typeof(NonNegativeDecimalGenerator)])]
  public void ValueIsRoundToMoneyPrecision(decimal value)
  {
    Money.TryCreate(value)
      .Match(() => -1, m => m.Value)
      .Should()
      .Be(Math.Round(value, Money.Precision));
  }

  [Property(Arbitrary = [typeof(NegativeDecimalGenerator)])]
  public void ReturnsNoneWhenValueIsNegative(decimal value)
  {
    Money.TryCreate(value)
      .Match(() => -1, m => m.Value)
      .Should()
      .Be(-1);
  }

  [Property]
  public void ValueIsZeroWhenCreatedWithZero()
  {
    Money.Zero.Value
      .Should()
      .Be(decimal.Zero);
  }
}
