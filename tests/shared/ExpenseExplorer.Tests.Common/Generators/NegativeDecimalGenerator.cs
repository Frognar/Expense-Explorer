namespace ExpenseExplorer.Tests.Common.Generators;

public static class NegativeDecimalGenerator
{
  public static Gen<decimal> Gen()
    =>
      from value in ArbMap.Default.GeneratorFor<decimal>()
      where value < 0
      select value;

  public static Arbitrary<decimal> Arbitrary() => Gen().ToArbitrary();
}
