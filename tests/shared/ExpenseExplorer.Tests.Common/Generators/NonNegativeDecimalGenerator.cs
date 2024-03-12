namespace ExpenseExplorer.Tests.Common.Generators;

public static class NonNegativeDecimalGenerator
{
  public static Arbitrary<decimal> NonNegativeDecimalGen()
  {
    return ArbMap.Default.ArbFor<decimal>()
      .Filter(d => d >= 0);
  }
}
