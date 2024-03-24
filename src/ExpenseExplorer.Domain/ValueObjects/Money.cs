namespace ExpenseExplorer.Domain.ValueObjects;

using System.Text.Json.Serialization;
using ExpenseExplorer.Domain.Exceptions;

public record Money
{
  public static readonly Money Zero = new(0);

  [JsonConstructor]
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
