namespace ExpenseExplorer.Domain.ValueObjects;

using ExpenseExplorer.Domain.Exceptions;
using FunctionalCore.Monads;

public record Quantity
{
  private Quantity(decimal value)
  {
    NonPositiveQuantityException.ThrowIfNotPositive(value);
    Value = Math.Round(value, 4);
  }

  public decimal Value { get; }

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
}
