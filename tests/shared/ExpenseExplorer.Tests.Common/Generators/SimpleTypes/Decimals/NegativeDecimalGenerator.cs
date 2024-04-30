namespace ExpenseExplorer.Tests.Common.Generators.SimpleTypes.Decimals;

public static class NegativeDecimalGenerator
{
  public static Gen<decimal> Gen()
    =>
      from value in ArbMap.Default.GeneratorFor<decimal>()
      select value == decimal.Zero ? -1 : -Math.Abs(value);

  public static Arbitrary<decimal> Arbitrary() => Gen().ToArbitrary();
}
