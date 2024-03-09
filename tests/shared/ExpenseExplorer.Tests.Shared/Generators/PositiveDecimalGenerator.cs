namespace ExpenseExplorer.Tests.Shared.Generators;

public class PositiveDecimalGenerator
{
  public static Arbitrary<decimal> PositiveDecimalGen()
  {
    return ArbMap.Default.ArbFor<decimal>()
      .Filter(d => d > 0)
      .Generator
      .ToArbitrary();
  }
}
