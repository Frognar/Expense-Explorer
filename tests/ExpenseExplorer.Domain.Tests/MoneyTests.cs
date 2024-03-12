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
    Money money = Money.Create(value);
    money.Value.Should().Be(Math.Round(value, 3));
  }
}
