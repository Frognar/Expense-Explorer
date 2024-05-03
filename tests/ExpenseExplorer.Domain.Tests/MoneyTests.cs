namespace ExpenseExplorer.Domain.Tests;

public class MoneyTests
{
  [Property(Arbitrary = [typeof(NegativeDecimalGenerator)])]
  public void ThrowsExceptionWhenValueIsNegative(decimal value)
  {
    Action act = () => _ = Money.Create(value);
    act.Should().Throw<NegativeMoneyException>();
  }

  [Property(Arbitrary = [typeof(NonNegativeDecimalGenerator)])]
  public void ValueIsRoundToMoneyPrecision(decimal value)
  {
    Money.Create(value).Value.Should().Be(Math.Round(value, Money.Precision));
  }

  [Property(Arbitrary = [typeof(NonNegativeDecimalGenerator)])]
  public void ValueIsRoundTo3DecimalPlacesWithRecordSyntax(decimal value)
  {
    Money money = Money.Zero with { Value = value };
    money.Value.Should().Be(Math.Round(value, Money.Precision));
  }

  [Property(Arbitrary = [typeof(NegativeDecimalGenerator)])]
  public void ThrowsExceptionWhenValueIsNegativeWithRecordSyntax(decimal value)
  {
    Action act = () => _ = Money.Zero with { Value = value };
    act.Should().Throw<NegativeMoneyException>();
  }

  [Property]
  public void ValueIsZeroWhenCreatedWithZero()
  {
    Money.Zero.Value.Should().Be(decimal.Zero);
  }
}
