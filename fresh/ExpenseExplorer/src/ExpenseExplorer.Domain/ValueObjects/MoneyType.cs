using DotMaybe;

namespace ExpenseExplorer.Domain.ValueObjects;

public readonly record struct MoneyType(decimal Amount, string Currency);

public static class Money
{
  public static Maybe<MoneyType> Create(decimal amount, string currency)
  {
    if (amount < decimal.Zero || string.IsNullOrWhiteSpace(currency))
    {
      return None.OfType<MoneyType>();
    }

    return Some.With(new MoneyType(amount, currency.Trim()));
  }
}
