namespace ExpenseExplorer.Tests.Common.Generators;

public static class PositiveDecimalGenerator
{
  public static Arbitrary<decimal> PositiveDecimalGen()
  {
    return ArbMap.Default.ArbFor<decimal>()
      .Filter(d => d > 0)
      .Generator
      .ToArbitrary();
  }
}
