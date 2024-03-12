namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Exceptions;
using ExpenseExplorer.Domain.ValueObjects;

public class QuantityTests
{
  [Property(Arbitrary = [typeof(NonPositiveDecimalGenerator)])]
  public void ThrowsExceptionWhenValueIsNotPositive(decimal value)
  {
    Action act = () => _ = Quantity.Create(value);
    act.Should().Throw<NonPositiveQuantityException>();
  }

  [Property(Arbitrary = [typeof(PositiveDecimalGenerator)])]
  public void ValueIsRoundTo4DecimalPlaces(decimal value)
  {
    Quantity.Create(value).Value.Should().Be(Math.Round(value, 4));
  }
}
