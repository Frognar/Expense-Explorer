namespace ExpenseExplorer.Domain.ValueObjects;

using ExpenseExplorer.Domain.Exceptions;
using FunctionalCore.Monads;

public record Money
{
  public static readonly Money Zero = new(0);

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

  public static Maybe<Money> TryCreate(decimal value)
  {
    return value < 0
      ? None.OfType<Money>()
      : Some.From(new Money(value));
  }
}
