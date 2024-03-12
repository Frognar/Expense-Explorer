namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Domain.ValueObjects;

public static class MoneyGenerator
{
  public static Arbitrary<Money> MoneyGen()
  {
    return NonNegativeDecimalGenerator.NonNegativeDecimalGen()
      .Generator
      .Select(Money.Create)
      .ToArbitrary();
  }
}
