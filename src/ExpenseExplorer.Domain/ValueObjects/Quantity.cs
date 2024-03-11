namespace ExpenseExplorer.Domain.ValueObjects;

using ExpenseExplorer.Domain.Exceptions;

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
}
