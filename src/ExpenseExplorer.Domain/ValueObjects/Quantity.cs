namespace ExpenseExplorer.Domain.ValueObjects;

using ExpenseExplorer.Domain.Exceptions;
using FunctionalCore.Monads;

public readonly record struct Quantity(decimal Value)
{
  public const int Precision = 4;

  private readonly decimal _value = RoundOrThrow(Value);

  public decimal Value
  {
    get => _value;
    init => _value = RoundOrThrow(value);
  }

  public static Quantity Create(decimal value)
  {
    return new Quantity(value);
  }

  public static Maybe<Quantity> TryCreate(decimal value)
  {
    return value <= 0
      ? None.OfType<Quantity>()
      : Some.From(new Quantity(value));
  }

  private static decimal RoundOrThrow(decimal value)
  {
    if (IsValid(value))
    {
      return Math.Round(value, Precision);
    }

    throw new NonPositiveQuantityException();
  }

  private static bool IsValid(decimal value)
  {
    return value > decimal.Zero;
  }
}
