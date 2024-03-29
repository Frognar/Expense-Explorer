namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Exceptions;
using ExpenseExplorer.Domain.ValueObjects;

public class MoneyTests
{
  [Property(Arbitrary = [typeof(NegativeDecimalGenerator)])]
  public void ThrowsExceptionWhenValueIsNegative(decimal value)
  {
    Action act = () => _ = Money.Create(value);
    act.Should().Throw<NegativeMoneyException>();
  }

  [Property(Arbitrary = [typeof(NonNegativeDecimalGenerator)])]
  public void ValueIsRoundTo3DecimalPlaces(decimal value)
  {
    Money.Create(value).Value.Should().Be(Math.Round(value, 3));
  }

  [Property]
  public void ValueIsZeroWhenCreatedWithZero()
  {
    Money.Zero.Value.Should().Be(0);
  }
}
