namespace ExpenseExplorer.Tests.Common.Generators.ComplexTypes;

using ExpenseExplorer.Domain.ValueObjects;
using ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Decimals;

public static class MoneyGenerator
{
  public static Gen<Money> Gen()
    =>
      from value in NonNegativeDecimalGenerator.Gen()
      select Money.Create(value);

  public static Arbitrary<Money> Arbitrary() => Gen().ToArbitrary();
}
