namespace ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Decimals;

public static class NonNegativeDecimalGenerator
{
  public static Gen<decimal> Gen()
    =>
      from value in ArbMap.Default.GeneratorFor<decimal>()
      select Math.Abs(value);

  public static Arbitrary<decimal> Arbitrary() => Gen().ToArbitrary();
}
