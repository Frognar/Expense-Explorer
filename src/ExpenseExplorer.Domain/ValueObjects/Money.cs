using DotMaybe;

namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct Money
{
  public const int Precision = 3;
  public static readonly Money Zero = new(decimal.Zero);

  private Money(decimal value)
  {
    Value = Math.Round(value, Precision);
  }

  public decimal Value { get; }

  public static Maybe<Money> TryCreate(decimal value)
  {
    return value >= decimal.Zero
      ? Some.With(new Money(value))
      : None.OfType<Money>();
  }
}
