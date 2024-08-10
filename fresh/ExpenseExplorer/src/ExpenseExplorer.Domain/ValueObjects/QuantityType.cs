using DotMaybe;

namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct QuantityType(decimal Value);

public static class Quantity
{
  public static Maybe<QuantityType> Create(decimal value)
    => value < decimal.Zero
      ? None.OfType<QuantityType>()
      : Some.With(new QuantityType(value));
}
