namespace ExpenseExplorer.Domain.ValueObjects;

using ExpenseExplorer.Domain.Exceptions;
using FunctionalCore.Monads;

public readonly record struct Money(decimal Value)
{
  public const int Precision = 3;
  public static readonly Money Zero = new(decimal.Zero);

  private readonly decimal _value = RoundOrThrow(Value);

  public decimal Value
  {
    get => _value;
    init => _value = RoundOrThrow(value);
  }

  public static Money Create(decimal value)
  {
    return new Money(value);
  }

  public static Maybe<Money> TryCreate(decimal value)
  {
    return IsValid(value)
      ? Some.From(new Money(value))
      : None.OfType<Money>();
  }

  private static decimal RoundOrThrow(decimal value)
  {
    if (IsValid(value))
    {
      return Math.Round(value, Precision);
    }

    throw new NegativeMoneyException();
  }

  private static bool IsValid(decimal value)
  {
    return value >= decimal.Zero;
  }
}
