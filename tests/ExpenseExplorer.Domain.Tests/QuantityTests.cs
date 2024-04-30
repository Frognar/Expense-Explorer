namespace ExpenseExplorer.Domain.Tests;

using ExpenseExplorer.Domain.Exceptions;
using ExpenseExplorer.Domain.ValueObjects;
using ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Decimals;

public class QuantityTests
{
  [Property(Arbitrary = [typeof(NonPositiveDecimalGenerator)])]
  public void ThrowsExceptionWhenValueIsNotPositive(decimal value)
  {
    Action act = () => _ = Quantity.Create(value);
    act.Should().Throw<NonPositiveQuantityException>();
  }

  [Property(Arbitrary = [typeof(PositiveDecimalGenerator)])]
  public void ValueIsRoundToQuantityPrecision(decimal value)
  {
    Quantity.Create(value).Value.Should().Be(Math.Round(value, Quantity.Precision));
  }

  [Property(Arbitrary = [typeof(NonPositiveDecimalGenerator)])]
  public void ThrowsExceptionWhenValueIsNotPositiveWithRecordSyntax(decimal value)
  {
    Action act = () => _ = Quantity.Create(1) with { Value = value };
    act.Should().Throw<NonPositiveQuantityException>();
  }

  [Property(Arbitrary = [typeof(PositiveDecimalGenerator)])]
  public void ValueIsRoundToQuantityPrecisionWithRecordSyntax(decimal value)
  {
    Quantity quantity = Quantity.Create(1) with { Value = value };
    quantity.Value.Should().Be(Math.Round(value, Quantity.Precision));
  }
}
