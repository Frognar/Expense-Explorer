namespace ExpenseExplorer.Domain.ValueObjects;

using ExpenseExplorer.Domain.Exceptions;

public record Money
{
  private Money(decimal value)
  {
    NegativeMoneyException.ThrowIfNegative(value);
    Value = Math.Round(value, 3);
  }

  public decimal Value { get; }

  public static Money Create(decimal value)
  {
    return new Money(value);
  }
}
