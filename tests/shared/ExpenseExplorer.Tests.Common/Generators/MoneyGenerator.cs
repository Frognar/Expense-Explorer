namespace ExpenseExplorer.Tests.Common.Generators;

using ExpenseExplorer.Domain.ValueObjects;

public static class MoneyGenerator
{
  public static Gen<Money> Gen()
    =>
      from value in NonNegativeDecimalGenerator.Gen()
      select Money.Create(value);

  public static Arbitrary<Money> Arbitrary() => Gen().ToArbitrary();
}
