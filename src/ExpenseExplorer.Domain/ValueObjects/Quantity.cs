namespace ExpenseExplorer.Domain.ValueObjects;

using DotMaybe;

public readonly record struct Quantity
{
  public const int Precision = 4;

  private Quantity(decimal value)
  {
    Value = Math.Round(value, Precision);
  }

  public decimal Value { get; }

  public static Maybe<Quantity> TryCreate(decimal value)
  {
    return value <= 0
      ? None.OfType<Quantity>()
      : Some.With(new Quantity(value));
  }
}
