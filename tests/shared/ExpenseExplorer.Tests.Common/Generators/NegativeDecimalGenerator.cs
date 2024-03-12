namespace ExpenseExplorer.Tests.Common.Generators;

public static class NegativeDecimalGenerator
{
  public static Arbitrary<decimal> NegativeDecimalGen()
  {
    return ArbMap.Default.ArbFor<decimal>()
      .Filter(d => d < 0);
  }
}
